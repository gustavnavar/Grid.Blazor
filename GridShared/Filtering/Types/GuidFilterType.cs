using System;
using System.Linq;
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
                case GridFilterType.NotEquals:
                case GridFilterType.Contains:
                case GridFilterType.StartsWith:
                case GridFilterType.EndsWidth:
                case GridFilterType.IsDuplicated:
                case GridFilterType.IsNotDuplicated:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            return value;
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
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
                    binaryExpression = GetCaseInsensitiveСomparation(string.Empty, toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.NotEquals:
                    binaryExpression = GetCaseInsensitiveСomparation("NotEquals", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.Contains:
                    binaryExpression = GetCaseInsensitiveСomparation("Contains", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.StartsWith:
                    binaryExpression = GetCaseInsensitiveСomparation("StartsWith", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.EndsWidth:
                    binaryExpression = GetCaseInsensitiveСomparation("EndsWith", toStringLeftExpr, valueExpr);
                    break;
                case GridFilterType.IsDuplicated:
                    Expression groupBy = GetGroupBy<T, Guid>(source, leftExpr);
                    MethodInfo methodInfo = typeof(Queryable).GetMethods()
                        .Single(r => r.Name == "Contains" && r.GetParameters().Length == 2)
                        .MakeGenericMethod(new Type[] { typeof(Guid) });
                    binaryExpression = Expression.Call(groupBy, methodInfo, toStringLeftExpr);
                    break;
                case GridFilterType.IsNotDuplicated:
                    groupBy = GetGroupBy<T, Guid>(source, leftExpr);
                    methodInfo = typeof(Queryable).GetMethods()
                        .Single(r => r.Name == "Contains" && r.GetParameters().Length == 2)
                        .MakeGenericMethod(new Type[] { typeof(Guid) });
                    var expresion = Expression.Call(groupBy, methodInfo, toStringLeftExpr);
                    binaryExpression = Expression.Not(expresion);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return binaryExpression;
        }

        private Expression GetCaseInsensitiveСomparation(string methodName, Expression leftExpr, Expression rightExpr)
        {
            //case insensitive compartion:
            MethodInfo miUpper = typeof(string).GetMethod("ToUpper", new Type[] { });
            MethodCallExpression upperValueExpr = Expression.Call(rightExpr, miUpper);
            MethodCallExpression upperFirstExpr = Expression.Call(leftExpr, miUpper);

            if (!string.IsNullOrEmpty(methodName))
            {
                if (methodName == "NotEquals")
                {
                    return Expression.NotEqual(upperFirstExpr, upperValueExpr);
                }
                else
                {
                    MethodInfo mi = typeof(string).GetMethod(methodName, new[] { typeof(string) });
                    if (mi == null)
                        throw new MissingMethodException("There is no method - " + methodName);
                    return Expression.Call(upperFirstExpr, mi, upperValueExpr);
                }
            }
            return Expression.Equal(upperFirstExpr, upperValueExpr);
        }

        #region OData

        public override string GetFilterExpression(string columnName, string value, GridFilterType filterType)
        {
            value = GetStringValue(value).ToLower();

            //base implementation of building filter expressions
            filterType = GetValidType(filterType);

            switch (filterType)
            {
                case GridFilterType.Equals:
                    return columnName + " eq " + value;
                case GridFilterType.NotEquals:
                    return columnName + " ne " + value;
                case GridFilterType.Contains:
                    return "contains(" + columnName + ", " + value + ")";
                case GridFilterType.StartsWith:
                    return "startswith(" + columnName + ", " + value + ")";
                case GridFilterType.EndsWidth:
                    return "endswith(" + columnName + ", " + value + ")";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}