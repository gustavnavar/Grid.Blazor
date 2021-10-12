using GridShared.Searching;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;

namespace GridCore.Searching
{
    /// <summary>
    ///     Object gets search settings from query string
    /// </summary>
    public class QueryStringSearchSettings : IGridSearchSettings
    {
        public const string DefaultSearchQueryParameter = "grid-search";
        private string _searchValue;

        #region Ctor's

        public QueryStringSearchSettings(IQueryDictionary<StringValues> query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;

            var search = Query.Get(DefaultSearchQueryParameter);
            if (search.Count > 0)
            {
                _searchValue = search[0];
            }
        }

        #endregion

        #region IGridSearchSettings Members

        public IQueryDictionary<StringValues> Query { get; }

        public string SearchValue
        {
            get { return _searchValue; }
        }

        #endregion
    }
}
