using System;
using System.Diagnostics;
using System.Linq;

namespace GridCore.Pagination
{
    /// <summary>
    ///     Cut's the current page from items collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagerGridItemsProcessor<T> : IGridItemsProcessor<T>
    {
        private readonly IGridPager _pager;
        private Func<IQueryable<T>, IQueryable<T>> _process;

        public PagerGridItemsProcessor(IGridPager pager)
        {
            _pager = pager;
        }

        public IQueryable<T> Process(IQueryable<T> items, int count)
        {
            _pager.Initialize(count);
            return Process(items);
        }

        #region IGridItemsProcessor<T> Members

        public IQueryable<T> Process(IQueryable<T> items)
        {
            if (_process != null)
                return _process(items);

            if (_pager.CurrentPage <= 0) return items; //incorrect page

            int skip = (_pager.CurrentPage - 1)*_pager.PageSize;
            return items.Skip(skip).Take(_pager.PageSize);
        }

        public void SetProcess(Func<IQueryable<T>, IQueryable<T>> process)
        {
            _process = process;
        }

        #endregion
    }
}