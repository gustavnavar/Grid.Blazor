namespace GridBlazor.Pagination
{
    /// <summary>
    ///     Settings grid items, based on current sorting settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PagerGridODataProcessor<T> : IGridODataProcessor<T>
    {
        private readonly ICGrid _grid;

        public PagerGridODataProcessor(ICGrid grid)
        {
            _grid = grid;
        }

        #region IGridODataProcessor<T> Members

        public string Process()
        {
            string result = "";
            if (_grid.EnablePaging)
            {                
                int numSkippedPages = _grid.Pager.CurrentPage - 1;
                if (numSkippedPages < 0)
                    numSkippedPages = 0;
                result = "$top=" + _grid.Pager.PageSize + "&$skip=" + _grid.Pager.PageSize * numSkippedPages;
            }
            return result;
        }

        #endregion
    }
}