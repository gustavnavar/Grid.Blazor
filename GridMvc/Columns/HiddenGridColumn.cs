using System;
using System.Linq.Expressions;

namespace GridMvc.Columns
{
    public class HiddenGridColumn<T, TDataType> : GridColumn<T, TDataType>
    {
        public HiddenGridColumn(Expression<Func<T, TDataType>> expression, ISGrid grid) : base(expression, grid)
        {
            Hidden = true;
        }     
    }
}