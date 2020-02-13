using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridMvc.Columns
{
    public class HiddenGridColumn<T, TDataType> : GridColumn<T, TDataType>
    {
        public HiddenGridColumn(Expression<Func<T, TDataType>> expression, ISGrid grid) : this(expression, null, grid)
        {
        }

        public HiddenGridColumn(Expression<Func<T, TDataType>> expression, IComparer<TDataType> comparer, ISGrid grid) 
            : base(expression, comparer, grid)
        {
            Hidden = true;
        }
    }
}