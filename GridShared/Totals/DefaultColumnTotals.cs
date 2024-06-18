using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Totals
{
    /// <summary>
    ///     Default Grid.Mvc search. Provides logic for searching items collection.
    /// </summary>
    public class DefaultColumnTotals<T, TData> : IColumnTotals<T>
    {
        private readonly Expression<Func<T, TData>> _expression;
        private readonly PropertyInfo _pi;

        public DefaultColumnTotals(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
            _pi = (PropertyInfo)((MemberExpression)expression.Body).Member;
        }

        #region IColumnFilter<T> Members

        public IList<string> GetNames()
        {
            try
            {
                List<string> names = new List<string>();
                Expression expression = _expression.Body;
                while (expression.NodeType != ExpressionType.Parameter)
                {
                    names.Add(((MemberExpression)expression).Member.Name);
                    expression = ((MemberExpression)expression).Expression;
                }
                names.Reverse();
                return names;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public bool IsNullable()
        {
            //detect nullable
            bool isNullable = _pi.PropertyType.GetTypeInfo().IsGenericType &&
                              _pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            return isNullable;
        }

        public Type GetPropertyType(bool isNullable)
        {
            //get target type:
            Type targetType = isNullable ? Nullable.GetUnderlyingType(_pi.PropertyType) : _pi.PropertyType;
            return targetType;
        }

        public Expression GetExpression(IList<string> names, ParameterExpression parameter)
        {
            //get first expression
            Expression firstExpression = parameter;
            foreach(var name in names)
            {
                firstExpression = Expression.Property(firstExpression, name);
            }

            return firstExpression;
        }

        public Expression GetCountExpression(Expression expression)
        {
            PropertyInfo count = _pi.PropertyType.GetProperty("Count");
            return Expression.Property(expression, count);
        }

        #endregion

        #region OData
        public string GetFullName()
        {
            var names = GetNames();

            string result = "";
            for (int i = 0; i < names.Count; i++)
            {
                result += names[i];
                if (i != names.Count - 1)
                    result += "/";
            }

            return result;
        }

        #endregion
    }
}