using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object builds filter expressions for text (string) grid columns
    /// </summary>
    public sealed class TextFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (String); }
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
            //Custom implementation of string filter type. Case insensitive compartion.

            filterType = GetValidType(filterType);
            object typedValue = GetTypedValue(value);
            if (typedValue == null)
                return null; //incorrent filter value;

            Expression valueExpr = Expression.Constant(typedValue);
            Expression binaryExpression;
            switch (filterType)
            {
                case GridFilterType.Equals:
                    binaryExpression = GetCaseInsensitiveСomparation(string.Empty, leftExpr, valueExpr);
                    break;
                case GridFilterType.NotEquals:
                    binaryExpression = GetCaseInsensitiveСomparation("NotEquals", leftExpr, valueExpr);
                    break;
                case GridFilterType.Contains:
                    binaryExpression = GetCaseInsensitiveСomparation("Contains", leftExpr, valueExpr);
                    break;
                case GridFilterType.StartsWith:
                    binaryExpression = GetCaseInsensitiveСomparation("StartsWith", leftExpr, valueExpr);
                    break;
                case GridFilterType.EndsWidth:
                    binaryExpression = GetCaseInsensitiveСomparation("EndsWith", leftExpr, valueExpr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return binaryExpression;
        }

        private Expression GetCaseInsensitiveСomparation(string methodName, Expression leftExpr, Expression rightExpr)
        {
            Type targetType = TargetType;
            //case insensitive compartion:
            MethodInfo miUpper = targetType.GetMethod("ToUpper", new Type[] {});
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
                    MethodInfo mi = targetType.GetMethod(methodName, new[] { typeof(string) });
                    if (mi == null)
                        throw new MissingMethodException("There is no method - " + methodName);
                    return Expression.Call(upperFirstExpr, mi, upperValueExpr);
                }
            }
            return Expression.Equal(upperFirstExpr, upperValueExpr);
        }
    }
}