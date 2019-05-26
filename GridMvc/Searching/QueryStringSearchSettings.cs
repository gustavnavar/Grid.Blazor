using GridShared.Searching;
using Microsoft.AspNetCore.Http;
using System;

namespace GridMvc.Searching
{
    /// <summary>
    ///     Object gets search settings from query string
    /// </summary>
    public class QueryStringSearchSettings : IGridSearchSettings
    {
        public const string DefaultSearchQueryParameter = "grid-search";
        public readonly IQueryCollection Query;
        private string _searchValue;

        #region Ctor's

        public QueryStringSearchSettings(IQueryCollection query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;

            string[] search = Query[DefaultSearchQueryParameter].Count > 0 ?
                Query[DefaultSearchQueryParameter].ToArray() : null;
            if (search != null && search.Length > 0)
            {
                _searchValue = search[0];
            }
        }

        #endregion

        #region IGridSearchSettings Members

        public override string SearchValue
        {
            get { return _searchValue; }
        }

        #endregion
    }
}
