using System;
using System.Linq.Expressions;

namespace GridBlazor.Columns
{
    public class HiddenGridColumn<T, TDataType> : GridColumn<T, TDataType>
    {
        public HiddenGridColumn(Expression<Func<T, TDataType>> expression, CGrid<T> grid) : base(expression, grid)
        {
            Hidden = true;
        }
    }
}
