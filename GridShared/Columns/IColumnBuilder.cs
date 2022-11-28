using GridShared.Sorting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Columns
{
    /// <summary>
    ///     Object which creates grid columns
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnBuilder<T>
    {
        bool DefaultSortEnabled { get; set; }
        GridSortMode DefaultGridSortMode { get; set; }
        bool DefaultFilteringEnabled { get; set; }

        /// <summary>
        ///     Creates column based on column expression
        /// </summary>
        /// <param name="expression">Column expression</param>
        /// <param name="hidden">Is column hidden</param>
        IGridColumn<T> CreateColumn<TDataType>(Expression<Func<T, TDataType>> expression, bool hidden);

        /// <summary>
        ///     Creates column based on column expression
        /// </summary>
        /// <param name="expression">Column expression</param>
        /// <param name="hidden">Is column hidden</param>
        IGridColumn<T> CreateColumn<TDataType>(Expression<Func<T, TDataType>> expression, IComparer<TDataType> comparer,
            bool hidden);

        /// <summary>
        ///     Creates column from property info using reflection
        /// </summary>
        IGridColumn<T> CreateColumn(PropertyInfo pi);
    }
}