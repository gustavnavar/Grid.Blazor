using System;
using System.Linq;

namespace GridCore
{
    /// <summary>
    ///     Preprocess items to display
    ///     This objects process initial collection of items in the Grid.Mvc (sorting, filtering, paging etc.)
    /// </summary>
    public interface IGridItemsProcessor<T>
    {
        IQueryable<T> Process(IQueryable<T> items);

        void SetProcess(Func<IQueryable<T>, IQueryable<T>> process);
    }

}