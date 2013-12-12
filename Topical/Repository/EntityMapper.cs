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
        private readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> _setters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
        private readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> _getters = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        public Document MapToDocument<TEntity>(TEntity entity)
        {
            var document = new Document();
            var type = typeof(TEntity);
            var properties = GetPropertyCache<TEntity>();
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
                Func<object, object> getter = GetPropertyGetter(propertyInfo);

                if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    IEnumerable<object> collection = ((IEnumerable)getter(entity)).Cast<object>();
                    foreach (var item in collection)
                    {
                        document.Add(new Field(fieldName, item.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                else
                {
                    IFieldable fieldable = null;
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var value = (String)getter(entity);
                        if (!String.IsNullOrEmpty(value))
                        {
                            fieldable = new Field(fieldName, value, store, index);
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        var value = (int)getter(entity);
                        fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetIntValue(value);
                    }
                    else if (propertyInfo.PropertyType == typeof(double))
                    {
                        var value = (double)getter(entity);
                        fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetDoubleValue(value);
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTimeOffset))
                    {
                        long value = ((DateTimeOffset)getter(entity)).UtcTicks;
                        fieldable = new NumericField(fieldName, store, index: index != Field.Index.NO).SetLongValue(value);
                    }
                    else
                    {
                        throw new NotSupportedException("Property type " + propertyInfo.PropertyType + " is not supported");
                    }

                    if (fieldable != null)
                    {
                        document.Add(fieldable);
                    }
                }
            }

            return document;
        }

        public TEntity MapFromDocument<TEntity>(Document document)
        {
            var properties = GetPropertyCache<TEntity>();
            var instance = Activator.CreateInstance<TEntity>();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string fieldName = propertyInfo.Name;
                IFieldable field = document.GetFieldable(fieldName);
                Action<object, object> setter = GetPropertySetter(propertyInfo);
                if (field == null)
                {
                    continue;
                }

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

        private PropertyInfo[] GetPropertyCache<TEntity>()
        {
            return _propertyCache.GetOrAdd(typeof(TEntity), type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        private Func<object, object> GetPropertyGetter(PropertyInfo info)
        {
            return _getters.GetOrAdd(info, (PropertyInfo v) =>
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
                return Expression.Lambda<Func<object, object>>(
                    Expression.Property(
                        Expression.Convert(instanceParameter, info.DeclaringType),
                        info
                    ),
                    instanceParameter
                ).Compile();
            });
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