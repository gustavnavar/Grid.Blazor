using GridShared.Columns;
using System.Collections.Generic;
using System.Linq;

namespace GridShared.Filtering
{
    public class DefaultFilterColumnCollection : List<ColumnFilterValue>, IFilterColumnCollection
    {
        public DefaultFilterColumnCollection() : base()
        {
        }

        public DefaultFilterColumnCollection(string name, GridFilterType type, string value) : this()
        {
            Add(new ColumnFilterValue(name, type, value));
        }

        public void Add(string name, GridFilterType type, string value)
        {
            Add(new ColumnFilterValue(name, type, value));
        }

        public IEnumerable<ColumnFilterValue> GetByColumn(IGridColumn column)
        {
            return this.Where(c => c.ColumnName.ToUpper() == column.Name?.ToUpper());
        }
    }
}
