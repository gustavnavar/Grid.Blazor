using GridShared.Pagination;
using System;
using System.Linq;

namespace GridCore.Pagination
{
    /// <summary>
    ///     Cut's the current page from items collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagerGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly ISGrid _grid;

        private Func<IQueryable<T>, IQueryable<T>> _process;

        public PagerGridItemsProcessor(ISGrid grid)
        {
            _grid = grid;
        }

        public IQueryable<T> Process(IQueryable<T> items, int count)
        {
            _grid.Pager.Initialize(count);
            return Process(items);
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            if (_process != null)
                return _process(items);

            if (_grid.PagingType == PagingType.Virtualization)
            {
                if (_grid.Pager == null || _grid.Pager.StartIndex < 0 || _grid.Pager.VirtualizedCount <= 0) return items.Take(0); //incorrect page

                return items.Skip(_grid.Pager.StartIndex).Take(_grid.Pager.VirtualizedCount);
            }
            else
            {
                if (_grid.Pager == null || _grid.Pager.CurrentPage <= 0) return items.Take(0); //incorrect page

                int skip = (_grid.Pager.CurrentPage - 1) * _grid.Pager.PageSize;
                return items.Skip(skip).Take(_grid.Pager.PageSize);
            }
        }

        public void SetProcess(Func<IQueryable<T>, IQueryable<T>> process)
        {
            _process = process;
        }

        #endregion
    }
}