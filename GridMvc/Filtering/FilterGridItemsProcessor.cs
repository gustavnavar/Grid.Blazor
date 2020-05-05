using GridShared.Columns;
using GridShared.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Filtering
{
    /// <summary>
    ///     Grid items filter proprocessor
    /// </summary>
    internal class FilterGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly ISGrid _grid;
        private IGridFilterSettings _settings;

        public FilterGridItemsProcessor(ISGrid grid, IGridFilterSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _grid = grid;
            _settings = settings;
        }

        public void UpdateSettings(IGridFilterSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            foreach (IGridColumn column in _grid.Columns)
            {
                var gridColumn = column as IGridColumn<T>;
                if (gridColumn == null) continue;
                if (gridColumn.Filter == null) continue;

                IEnumerable<ColumnFilterValue> options = _settings.IsInitState(column)
                                                             ? new List<ColumnFilterValue>
                                                                 {
                                                                     column.InitialFilterSettings
                                                                 }
                                                             : _settings.FilteredColumns.GetByColumn(column);

                items = ((IColumnFilter<T>)gridColumn.Filter).ApplyFilter(items, options);
            }
            return items;
        }

        #endregion
    }
}