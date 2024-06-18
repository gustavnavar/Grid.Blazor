using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GridShared.Grouping
{
    public class DefaultColumnGroup<T, TData> : IColumnGroup<T>
    {
        private readonly Expression<Func<T, TData>> _expression;
        
        public Func<object, Task<string>> GroupLabel { get; set; }

        public DefaultColumnGroup(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
        }

        public Expression<Func<T, object>> GetColumnExpression()
        {
            ParameterExpression parameter = _expression.Parameters[0];
            Expression expression = _expression.Body;

            //detect nullable
            var pi = (PropertyInfo)((MemberExpression)expression).Member;
            bool isNullable = pi.PropertyType.GetTypeInfo().IsGenericType &&
                              pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            Type targetType = isNullable ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;

            if (targetType.IsGenericType && targetType.Name == "ICollection`1")
            {
                PropertyInfo count = pi.PropertyType.GetProperty("Count");
                expression = Expression.Property(expression, count);
                expression = Expression.Convert(expression, typeof(object));
            }
            else if (isNullable)
            {
                Expression hasValueExpr = Expression.Property(expression, pi.PropertyType.GetProperty("HasValue"));
                expression = Expression.Property(expression, pi.PropertyType.GetProperty("Value"));
                expression = Expression.Convert(expression, typeof(object));
                expression = Expression.Condition(hasValueExpr, expression, Expression.Constant(null));
            }
            else
            {
                expression = Expression.Convert(expression, typeof(object));
            }

            //return filter expression 
            return Expression.Lambda<Func<T, object>>(expression, parameter);
        }

        public IQueryable<object> GetColumnValues(IQueryable<T> items)
        {
            var lambda = GetColumnExpression();
            return items.Select(lambda);
        }

        public IQueryable<T> ApplyFilter(IQueryable<T> items, object value)
        {
            ParameterExpression parameter = _expression.Parameters[0];
            Expression expression = _expression.Body;

            //detect nullable
            var pi = (PropertyInfo)((MemberExpression)expression).Member;
            bool isNullable = pi.PropertyType.GetTypeInfo().IsGenericType &&
                              pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (isNullable)
            {
                Expression hasValueExpr = Expression.Property(expression, pi.PropertyType.GetProperty("HasValue"));
                if (value == null)
                {
                    expression = Expression.Not(hasValueExpr);
                }
                else
                {   
                    expression = Expression.Property(expression, pi.PropertyType.GetProperty("Value"));
                    expression = Expression.Equal(expression, Expression.Constant(value));
                    expression = Expression.AndAlso(hasValueExpr, expression);
                }                 
            }
            else 
            {
                expression = Expression.Equal(expression, Expression.Constant(value));
            }

            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return items.Where(lambda);
        }
    }
}
