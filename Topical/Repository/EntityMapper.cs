using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lucene.Net.Documents;

namespace Topical.Repository
{
    public class EntityMapper
    {
        private static readonly Type[] _supportedTypes = new[] { typeof(string), typeof(int), typeof(DateTimeOffset), typeof(double) };
        private readonly ConcurrentDictionary<Type, IDictionary> _entityMapping = new ConcurrentDictionary<Type, IDictionary>();
        private readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> _setters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        public Document MapToDocument<TEntity>(TEntity entity)
        {
            var type = typeof(TEntity);
            var mapping = (Dictionary<PropertyInfo, Func<TEntity, ICollection<FieldResult>>>)_entityMapping.GetOrAdd(type, e => CreateEntityMapping<TEntity>());

            var document = new Document();
            foreach (var item in mapping)
            {
                PropertyInfo property = item.Key;
                ICollection<FieldResult> fieldResults = item.Value(entity);
                if (fieldResults != null)
                {
                    foreach (var fieldResult in fieldResults)
                    {
                        document.Add(fieldResult.Fieldable);
                    }
                }
            }

            return document;
        }

        public TEntity MapFromDocument<TEntity>(Document document)
        {
            var type = typeof(TEntity);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var instance = Activator.CreateInstance<TEntity>();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string fieldName = propertyInfo.Name;
                IFieldable field = document.GetFieldable(fieldName);
                Action<object, object> setter = GetPropertySetter(propertyInfo);

                if (propertyInfo.PropertyType == typeof(string))
                {
                    setter(instance, field.StringValue);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    setter(instance, Convert.ToInt32(field.StringValue));
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    setter(instance, Convert.ToDouble(field.StringValue));
                }
                else if (propertyInfo.PropertyType == typeof(DateTimeOffset))
                {
                    DateTimeOffset dateTime;
                    if (DateTimeOffset.TryParse(field.StringValue, out dateTime))
                    {
                        setter(instance, dateTime);
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var collection = (ICollection<string>)Activator.CreateInstance(propertyInfo.PropertyType);
                    setter(instance, collection);

                    var fields = document.GetValues(fieldName);
                    foreach (string value in fields)
                    {
                        collection.Add(value);
                    }
                }
            }
            return instance;
        }

        private IDictionary CreateEntityMapping<TEntity>()
        {
            var type = typeof(TEntity);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dictionary = new Dictionary<PropertyInfo, Func<TEntity, ICollection<FieldResult>>>(properties.Length);
            foreach (PropertyInfo propertyInfo in properties)
            {
                var indexMetadata = propertyInfo.GetCustomAttribute<IndexMetadata>();

                // Create a getter lambda () => (object)obj.Property;
                var index = indexMetadata == null ? Field.Index.ANALYZED : indexMetadata.Index;
                var store = indexMetadata == null ? Field.Store.YES : indexMetadata.Store;

                if (indexMetadata != null && indexMetadata.IsKey)
                {
                    index = Field.Index.NOT_ANALYZED;
                }

                string fieldName = propertyInfo.Name;
                Func<TEntity, ICollection<FieldResult>> fieldMapper = entity =>
                {
                    FieldResult fieldResult = null;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var getter = GetPropertyGetter<TEntity, string>(propertyInfo.Name);
                        var value = getter(entity);
                        if (String.IsNullOrEmpty(value))
                        {
                            return null;
                        }

                        fieldResult = new FieldResult
                        {
                            Value = value,
                            Fieldable = new Field(fieldName, value, store, index)
                        };
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        var getter = GetPropertyGetter<TEntity, int>(propertyInfo.Name);
                        var value = getter(entity);

                        fieldResult = new FieldResult
                        {
                            Value = value,
                            Fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetIntValue(value)
                        };
                    }
                    else if (propertyInfo.PropertyType == typeof(double))
                    {
                        var getter = GetPropertyGetter<TEntity, double>(propertyInfo.Name);
                        var value = getter(entity);

                        fieldResult = new FieldResult
                        {
                            Value = value,
                            Fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetDoubleValue(value)
                        };
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTimeOffset))
                    {
                        var getter = GetPropertyGetter<TEntity, DateTimeOffset>(propertyInfo.Name);
                        long value = getter(entity).UtcTicks;
                        fieldResult = new FieldResult
                        {
                            Value = value,
                            Fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetLongValue(value)
                        };
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        var getter = GetPropertyGetter<TEntity, IEnumerable>(propertyInfo.Name);
                        IEnumerable value = getter(entity);

                        return value.Cast<object>()
                                    .Select(v => new FieldResult
                                    {
                                        Fieldable = new Field(fieldName, v.ToString(), Field.Store.YES, Field.Index.ANALYZED),
                                        Value = v
                                    }).ToList();
                    }

                    if (fieldResult != null)
                    {
                        return new[] { fieldResult };
                    }
                    throw new InvalidOperationException();
                };

                dictionary.Add(propertyInfo, fieldMapper);
            }
            return dictionary;
        }

        public static Func<TEntity, TProperty> GetPropertyGetter<TEntity, TProperty>(string propertyName)
        {
            var instanceParameter = Expression.Parameter(typeof(TEntity), "target");
            Expression call = Expression.Property(instanceParameter, propertyName);
            if (typeof(TProperty) == typeof(object))
            {
                call = Expression.Convert(call, typeof(object));
            }

            return Expression.Lambda<Func<TEntity, TProperty>>(call, instanceParameter)
                             .Compile();
        }

        public Action<object, object> GetPropertySetter(PropertyInfo info)
        {
            return _setters.GetOrAdd(info, v =>
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target"),
                                propertyValue = Expression.Parameter(typeof(object), "value");
                Expression call = Expression.Property(Expression.Convert(instanceParameter, info.DeclaringType), info.Name);
                Expression assignment = Expression.Assign(call, Expression.Convert(propertyValue, info.PropertyType));
                return Expression.Lambda<Action<object, object>>(assignment, instanceParameter, propertyValue)
                                 .Compile();
            });
        }

        private sealed class FieldResult
        {
            public IFieldable Fieldable { get; set; }

            public object Value { get; set; }
        }
    }
}