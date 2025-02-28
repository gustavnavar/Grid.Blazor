using GridShared.Columns;
using GridShared.Sorting;
using System;
using System.Linq;

namespace GridCore.Sorting
{
    /// <summary>
    ///     Settings grid items, based on current sorting settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SortGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly ISGrid _grid;
        private IGridSortSettings _settings;
        private Func<IQueryable<T>, IQueryable<T>> _process;

        public SortGridItemsProcessor(ISGrid grid, IGridSortSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _grid = grid;
            _settings = settings;
        }

        public void UpdateSettings(IGridSortSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            if (_process != null)
                return _process(items);

            if (items == null)
                return items;

            if (_settings.SortValues?.Count() > 0)
            {
                var sortedColumns = _settings.SortValues.OrderBy(r => r.Id).ToList();

                var gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == sortedColumns[0].ColumnName) as IGridColumn<T>;
                if(gridColumn == null || gridColumn.NotDbMapped)
                    return items;
                items = gridColumn.Orderers.FirstOrDefault().ApplyOrder(items, sortedColumns[0].Direction);
                for (int i = 1; i < gridColumn.Orderers.Count(); i++)
                {
                    var orderer = gridColumn.Orderers.ElementAt(i);
                    items = orderer.ApplyThenBy(items, GridSortDirection.Ascending);
                }

                if(sortedColumns.Count() > 1)
                {
                    for(int i = 1; i < sortedColumns.Count(); i++)
                    {
                        gridColumn = _grid.Columns.FirstOrDefault(r => r.Name == sortedColumns[i].ColumnName) as IGridColumn<T>;
                        items = gridColumn.Orderers.FirstOrDefault().ApplyThenBy(items, sortedColumns[i].Direction);

                        for (int j = 1; j < gridColumn.Orderers.Count(); j++)
                        {
                            var orderer = gridColumn.Orderers.ElementAt(j);
                            items = orderer.ApplyThenBy(items, GridSortDirection.Ascending);
                        }
                    }
                }

                if (string.IsNullOrEmpty(_settings.ColumnName))
                    return items;
                //determine gridColumn sortable:
                gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == _settings.ColumnName) as IGridColumn<T>;
                if (gridColumn == null || !gridColumn.SortEnabled || gridColumn.NotDbMapped)
                    return items;
                foreach (var columnOrderer in gridColumn.Orderers)
                {
                    items = columnOrderer.ApplyThenBy(items, _settings.Direction);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_settings.ColumnName))
                    return items;
                //determine gridColumn sortable:
                var gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == _settings.ColumnName) as IGridColumn<T>;
                if (gridColumn == null || !gridColumn.SortEnabled || gridColumn.NotDbMapped)
                    return items;
                foreach (var columnOrderer in gridColumn.Orderers)
                {
                    items = columnOrderer.ApplyOrder(items, _settings.Direction);
                }
            }
            return items;
        }

        public void SetProcess(Func<IQueryable<T>, IQueryable<T>> process)
        {
            _process = process;
        }

        #endregion
    }
}