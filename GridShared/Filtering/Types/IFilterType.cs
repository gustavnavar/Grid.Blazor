﻿using System;
using System.Linq.Expressions;

namespace GridShared.Filtering.Types
{
    public interface IFilterType
    {
        /// <summary>
        ///     .Net type name for current filter
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        ///     Sanitize filter type for specific column data type
        /// </summary>
        /// <param name="type">Filter type (equals, contains etc)</param>
        /// <returns>Sanitized filter type</returns>
        GridFilterType GetValidType(GridFilterType type);

        /// <summary>
        ///     Return typed object from text representation (query string parameter value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object GetTypedValue(string value);

        Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType);

        string GetFilterExpression(string columnName, string value, GridFilterType filterType);

        string GetStringValue(string value);
    }
}