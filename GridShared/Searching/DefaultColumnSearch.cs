using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Searching
{
    /// <summary>
    ///     Default Grid.Mvc search. Provides logic for searching items collection.
    /// </summary>
    public class DefaultColumnSearch<T, TData> : IColumnSearch<T>
    {
        private readonly Expression<Func<T, TData>> _expression;

        public DefaultColumnSearch(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
        }

        #region IColumnFilter<T> Members

        public Expression GetExpression(string value, bool onlyTextColumns, ParameterExpression parameter, 
            MethodInfo removeDiacritics = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                var pi = (PropertyInfo)((MemberExpression)_expression.Body).Member;
                List<string> names = new List<string>();
                Expression expression = _expression.Body;
                while (expression.NodeType != ExpressionType.Parameter)
                {
                    names.Add(((MemberExpression)expression).Member.Name);
                    expression = ((MemberExpression)expression).Expression;
                }

                //detect nullable
                bool isNullable = pi.PropertyType.GetTypeInfo().IsGenericType &&
                                  pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                //get target type:
                Type targetType = isNullable ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;

                // bool columns are not searched as a workaround until the final release of EF Core 3.0
                if (targetType == typeof(bool)) return null;

                if (onlyTextColumns && targetType != typeof(string))
                    return null;

                //get first expression
                Expression firstExpression = parameter;
                Expression binaryExpression = null;
                for (int i = names.Count - 1; i >= 0; i--)
                {
                    firstExpression = Expression.Property(firstExpression, names[i]);

                    var nestedPi = (PropertyInfo)((MemberExpression)firstExpression).Member;

                    //detect nullable
                    bool nestedIsNullable = nestedPi.PropertyType.GetTypeInfo().IsGenericType &&
                                      nestedPi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                    //get target type:
                    Type nestedTargetType = nestedIsNullable ? Nullable.GetUnderlyingType(nestedPi.PropertyType) : nestedPi.PropertyType;

                    // Check for null on nested properties and not value type (string and objects are reference type)
                    // It's ok for ORM, but throw exception in linq to objects
                    if (nestedIsNullable || !nestedTargetType.IsValueType)
                    {
                        binaryExpression = binaryExpression == null ?
                            Expression.NotEqual(firstExpression, Expression.Constant(null)) :
                            Expression.AndAlso(binaryExpression, Expression.NotEqual(firstExpression, Expression.Constant(null)));
                    }
                }


                if (targetType != typeof(string))
                {
                    if (isNullable)
                    {
                        //add additional filter condition for check items on NULL with invoring "HasValue" method.
                        //for example: result of this expression will like - c=> c.HasValue && c.Value = 3
                        if (binaryExpression == null)
                            binaryExpression = Expression.Property(firstExpression, pi.PropertyType.GetProperty("HasValue"));
                        else
                            binaryExpression = Expression.AndAlso(binaryExpression, Expression.Property(firstExpression, pi.PropertyType.GetProperty("HasValue")));

                        firstExpression = Expression.Property(firstExpression, pi.PropertyType.GetProperty("Value"));
                    }
                    // add ToString method to non string columns
                    MethodInfo toString = targetType.GetMethod("ToString", Type.EmptyTypes);
                    firstExpression = Expression.Call(firstExpression, toString);
                }

                MethodInfo toUpper = typeof(string).GetMethod("ToUpper", new Type[] { });
                firstExpression = Expression.Call(firstExpression, toUpper);
                Expression valueExpression = Expression.Constant(value);
                valueExpression = Expression.Call(valueExpression, toUpper);

                if (removeDiacritics != null)
                {
                    firstExpression = Expression.Call(removeDiacritics, firstExpression);
                    valueExpression = Expression.Call(removeDiacritics, valueExpression);
                }

                MethodInfo miContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                Expression containsExpression = Expression.Call(firstExpression, miContains, valueExpression);

                if (containsExpression == null) return null;

                if (binaryExpression == null)
                    return containsExpression;
                else
                    return Expression.AndAlso(binaryExpression, containsExpression);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}