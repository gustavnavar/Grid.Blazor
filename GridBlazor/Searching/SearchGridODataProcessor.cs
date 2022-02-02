using GridShared.Searching;
using System;
using System.Net;

namespace GridBlazor.Searching
{
    internal class SearchGridODataProcessor<T> : IGridODataProcessor<T>
    {
        private readonly ICGrid _grid;
        private IGridSearchSettings _settings;

        public SearchGridODataProcessor(ICGrid grid, IGridSearchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _grid = grid;
            _settings = settings;
        }

        public void UpdateSettings(IGridSearchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
        }

        #region IGridODataProcessor<T> Members

        public string Process()
        {
            string result = "";
            if (_grid.SearchOptions.Enabled && !string.IsNullOrWhiteSpace(_settings.SearchValue))
            {
                result = "$search='" + WebUtility.UrlEncode(_settings.SearchValue.Replace("'", "''")) + "'";
            }
            return result;         
        }

        #endregion
    }
}
