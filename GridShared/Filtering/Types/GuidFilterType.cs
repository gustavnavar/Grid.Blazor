using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Guid columns
    /// </summary>
    public sealed class GuidFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Guid); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.Contains:
                case GridFilterType.StartsWith:
                case GridFilterType.EndsWidth:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            return value;
        }

        public override Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType)
        {
            //base implementation of building filter expressions
            filterType = GetValidType(filterType);

            object typedValue = GetTypedValue(value);
            if (typedValue == null)
                return null; //incorrent filter value;

            Expression valueExpr = Expression.Constant(typedValue);

            Type targetType = TargetType;
            MethodInfo toString = targetType.GetMethod("ToString", new Type[] { });
            MethodCallExpression toStringLeftExpr = Expression.Call(leftExpr, toString);

            Expression binaryExpression;
            switch (filterType)
            {
                case GridFilterType.Equals:
                    binaryExpression = GetCaseInsensitiveСompartion(string.Empty, toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.Contains:
                    binaryExpression = GetCaseInsensitiveСompartion("Contains", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.StartsWith:
                    binaryExpression = GetCaseInsensitiveСompartion("StartsWith", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.EndsWidth:
                    binaryExpression = GetCaseInsensitiveСompartion("EndsWith", toStringLeftExpr, valueExpr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return binaryExpression;
        }

        private Expression GetCaseInsensitiveСompartion(string methodName, Expression leftExpr, Expression rightExpr)
        {
            //case insensitive compartion:
            MethodInfo miUpper = typeof(string).GetMethod("ToUpper", new Type[] { });
            MethodCallExpression upperValueExpr = Expression.Call(rightExpr, miUpper);
            MethodCallExpression upperFirstExpr = Expression.Call(leftExpr, miUpper);

            if (!string.IsNullOrEmpty(methodName))
            {
                MethodInfo mi = typeof(string).GetMethod(methodName, new[] { typeof(string) });
                if (mi == null)
                    throw new MissingMethodException("There is no method - " + methodName);
                return Expression.Call(upperFirstExpr, mi, upperValueExpr);
            }
            return Expression.Equal(upperFirstExpr, upperValueExpr);
        }
    }
}