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
                case GridFilterType.IsNull:
                case GridFilterType.IsNotNull:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            return value;
        }

        public override Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType,
            MethodInfo removeDiacritics)
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
                case GridFilterType.IsNull:
                    binaryExpression = GetCaseInsensitiveСomparation(string.Empty, leftExpr, valueExpr, removeDiacritics);
                    break;
                case GridFilterType.NotEquals:
                case GridFilterType.IsNotNull:
                    binaryExpression = GetCaseInsensitiveСomparation("NotEquals", leftExpr, valueExpr, removeDiacritics);
                    break;
                case GridFilterType.Contains:
                    binaryExpression = GetCaseInsensitiveСomparation("Contains", leftExpr, valueExpr, removeDiacritics);
                    break;
                case GridFilterType.StartsWith:
                    binaryExpression = GetCaseInsensitiveСomparation("StartsWith", leftExpr, valueExpr, removeDiacritics);
                    break;
                case GridFilterType.EndsWidth:
                    binaryExpression = GetCaseInsensitiveСomparation("EndsWith", leftExpr, valueExpr, removeDiacritics);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return binaryExpression;
        }

        private Expression GetCaseInsensitiveСomparation(string methodName, Expression leftExpr, Expression rightExpr,
            MethodInfo removeDiacritics)
        {
            Type targetType = TargetType;
            //case insensitive compartion:
            MethodInfo miUpper = targetType.GetMethod("ToUpper", new Type[] {});
            MethodCallExpression upperValueExpr = Expression.Call(rightExpr, miUpper);
            MethodCallExpression upperFirstExpr = Expression.Call(leftExpr, miUpper);

            if (removeDiacritics != null)
            {
                upperFirstExpr = Expression.Call(removeDiacritics, upperFirstExpr);
                upperValueExpr = Expression.Call(removeDiacritics, upperValueExpr);
            }

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

        #region OData

        public override string GetFilterExpression(string columnName, string value, GridFilterType filterType)
        {
            value = GetStringValue(value).ToLower();
            columnName = "tolower(" + columnName + ")";

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
                case GridFilterType.IsNull:
                    return columnName + " eq null";
                case GridFilterType.IsNotNull:
                    return columnName + " ne null";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetStringValue(string value)
        {
            return "'" + value.Replace("'", "''") + "'";
        }

        #endregion
    }
}