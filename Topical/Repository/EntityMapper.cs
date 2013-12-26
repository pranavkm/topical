using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

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
                var indexMetadata = propertyInfo.GetCustomAttribute<IndexAttribute>();

                bool index = false;
                var store = Field.Store.YES;
                Field.Index indexValue = Field.Index.NO;
                if (indexMetadata != null)
                {
                    index = true;
                    indexValue = indexMetadata.Analyzed ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED;
                    store = indexMetadata.Store ? Field.Store.YES : Field.Store.NO;
                }

                string fieldName = propertyInfo.Name;
                Func<object, object> getter = GetPropertyGetter(propertyInfo);

                IFieldable fieldable = null;
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var value = (String)getter(entity);
                    if (!String.IsNullOrEmpty(value))
                    {
                        fieldable = new Field(fieldName, value, store, indexValue);
                    }
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    var value = (int)getter(entity);
                    fieldable = new NumericField(fieldName, store, index).SetIntValue(value);
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    var value = (double)getter(entity);
                    fieldable = new NumericField(fieldName, store, index).SetDoubleValue(value);
                }
                else if (propertyInfo.PropertyType == typeof(DateTimeOffset))
                {
                    long value = ((DateTimeOffset)getter(entity)).UtcTicks;
                    fieldable = new NumericField(fieldName, store, index).SetLongValue(value);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    IEnumerable enumerable = (IEnumerable)getter(entity);
                    if (enumerable != null)
                    {
                        foreach (object item in enumerable)
                        {
                            document.Add(new Field(fieldName, item.ToString(), store, indexValue));
                        }
                    }
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
                    long ticks = Convert.ToInt64(field.StringValue);
                    setter(instance, new DateTimeOffset(ticks, TimeSpan.Zero));
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

        public Query GetLookupTerm<TEntity>(TEntity entity)
        {
            var indexProperties = GetPropertyCache<TEntity>().Where(IsKeyProperty);
            var booleanQuery = new BooleanQuery();

            foreach (var property in indexProperties)
            {
                var getter = GetPropertyGetter(property);
                var value = (string)getter(entity);
                var term = new Term(property.Name, value);
                booleanQuery.Add(new TermQuery(term), Occur.MUST);
            }

            return booleanQuery;
        }

        private PropertyInfo[] GetPropertyCache<TEntity>()
        {
            return _propertyCache.GetOrAdd(typeof(TEntity), type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        private Func<object, object> GetPropertyGetter(PropertyInfo info)
        {
            // Create a getter lambda () => (object)obj.Property;
            return _getters.GetOrAdd(info, (PropertyInfo v) =>
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
                return
                    Expression.Lambda<Func<object, object>>(
                        Expression.Convert(
                            Expression.Property(
                                Expression.Convert(instanceParameter, info.DeclaringType),
                                info
                            ),
                            typeof(object)
                        ),
                        instanceParameter
                    ).Compile();
            });
        }

        private Action<object, object> GetPropertySetter(PropertyInfo info)
        {
            // Create a setter lambda (value) => obj.Property = (TProperty)value;
            return _setters.GetOrAdd(info, v =>
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target"),
                                    propertyValue = Expression.Parameter(typeof(object), "value");
                return
                    Expression.Lambda<Action<object, object>>(
                        Expression.Assign(
                            Expression.Property(
                                Expression.Convert(instanceParameter, info.DeclaringType),
                                info),
                            Expression.Convert(propertyValue, info.PropertyType)),
                        instanceParameter,
                        propertyValue
                    ).Compile();
            });
        }

        private bool IsKeyProperty(PropertyInfo property)
        {
            return property.IsDefined(typeof(IndexAttribute));
        }

        private sealed class FieldResult
        {
            public IFieldable Fieldable { get; set; }

            public object Value { get; set; }
        }
    }
}