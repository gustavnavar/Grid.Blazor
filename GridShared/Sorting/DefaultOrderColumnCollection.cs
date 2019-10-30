using GridShared.Columns;
using System.Collections.Generic;
using System.Linq;

namespace GridShared.Sorting
{
    public class DefaultOrderColumnCollection : List<ColumnOrderValue>, IOrderColumnCollection
    {
        public DefaultOrderColumnCollection() : base()
        {
        }

        public DefaultOrderColumnCollection(string name, GridSortDirection direction, int id) : this()
        {
            Add(new ColumnOrderValue(name, direction, id));
        }

        public void Add(string name, GridSortDirection direction, int id)
        {
            Add(new ColumnOrderValue(name, direction, id));
        }

        public IEnumerable<ColumnOrderValue> GetByColumn(IGridColumn column)
        {
            return this.Where(c => c.ColumnName.ToUpper() == column.Name?.ToUpper());
        }
    }
}
