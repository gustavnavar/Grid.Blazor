using GridCore;
using GridCore.Columns;
using GridShared.Columns;
using GridShared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridMvc.Columns
{
    /// <summary>
    ///     Default grid columns builder. Creates the columns from expression
    /// </summary>
    internal class DefaultColumnBuilder<T> : DefaultColumnCoreBuilder<T>
    {
        public DefaultColumnBuilder(ISGrid grid, IGridAnnotationsProvider annotations) : base(grid, annotations)
        {
        }

        #region IColumnBuilder<T> Members

        public override IGridColumn<T> CreateColumn<TDataType>(Expression<Func<T, TDataType>> constraint, IComparer<TDataType> comparer,
            bool hidden)
        {
            bool isExpressionOk = constraint == null || constraint.Body as MemberExpression != null;
            if (isExpressionOk)
            {
                var column = new GridColumn<T, TDataType>(constraint, comparer, _grid);
                column.Hidden = hidden;
                return column;
            }
            throw new NotSupportedException(string.Format("Expression '{0}' not supported by grid", constraint));
        }

        #endregion
    }
}