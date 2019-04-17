using System.Linq;

namespace GridMvc
{
    /// <summary>
    ///     Preprocess items to display
    ///     This objects process initial collection of items in the Grid.Mvc (sorting, filtering, paging etc.)
    /// </summary>
    public interface IGridItemsProcessor<T>
    {
        IQueryable<T> Process(IQueryable<T> items);
    }

}