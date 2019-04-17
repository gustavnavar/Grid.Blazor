using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;

namespace GridBlazor.Pagination
{
    /// <summary>
    ///     Default grid pager implementation
    /// </summary>
    public class GridPager: IGridPager
    {
        public const int DefaultMaxDisplayedPages = 5;
        public const int DefaultPageSize = 20;

        public const string DefaultPageQueryParameter = "grid-page";

        private IQueryDictionary<StringValues> _query;
        private CustomQueryStringBuilder _queryBuilder;
        private int _currentPage;

        private int _itemsCount;
        private int _maxDisplayedPages;
        private int _pageSize;

        #region ctor's

        public GridPager()
        {
            _query = new QueryDictionary<StringValues>();
            _currentPage = -1;
            _queryBuilder = new CustomQueryStringBuilder(_query);

            ParameterName = DefaultPageQueryParameter;
            MaxDisplayedPages = MaxDisplayedPages;
            PageSize = DefaultPageSize;
        }

        public GridPager(IQueryDictionary<StringValues> query) : this()
        {
            _query = query;
            _queryBuilder = new CustomQueryStringBuilder(_query);
        }

        public GridPager(IQueryDictionary<StringValues> query, int page) : this(query)
        {
            CurrentPage = page;
        }
        #endregion

        public IQueryDictionary<StringValues> Query
        {
            get { return _query; }
            set
            {
                _query = value;
                _queryBuilder = new CustomQueryStringBuilder(_query);
            }
        }

        #region IGridPager members

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RecalculatePages();
            }
        }

        public int CurrentPage
        {
            get
            {
                if (_currentPage >= 0) return _currentPage;
                string currentPageString = _query.Get(ParameterName).ToString() ?? "1";
                if (!int.TryParse(currentPageString, out _currentPage))
                    _currentPage = 1;
                if (_currentPage > PageCount)
                    _currentPage = PageCount;
                return _currentPage;
            }
            protected internal set
            {
                _currentPage = value;
                //if (_currentPage > PageCount)
                //    _currentPage = PageCount;
                RecalculatePages();
            }
        }

        #endregion

        /// <summary>
        ///     Query string parameter name, that determine current displaying page
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        ///     Total items of the initial collection
        /// </summary>
        public virtual int ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                _itemsCount = value;
                RecalculatePages();
            }
        }

        public int MaxDisplayedPages
        {
            get { return _maxDisplayedPages == 0 ? DefaultMaxDisplayedPages : _maxDisplayedPages; }
            set
            {
                _maxDisplayedPages = value;
                RecalculatePages();
            }
        }

        /// <summary>
        ///     Total pages count
        /// </summary>
        public int PageCount { get; protected set; }

        public virtual void Initialize(int count)
        {
            ItemsCount = count; //take total items count from collection
        }

        protected virtual void RecalculatePages()
        {
            if (ItemsCount == 0)
            {
                PageCount = 0;
                return;
            }
            PageCount = (int) (Math.Ceiling(ItemsCount/(double) PageSize));

            if (CurrentPage > PageCount)
                CurrentPage = PageCount;

            StartDisplayedPage = (CurrentPage - MaxDisplayedPages/2) < 1 ? 1 : CurrentPage - MaxDisplayedPages/2;
            EndDisplayedPage = (CurrentPage + MaxDisplayedPages/2) > PageCount
                                   ? PageCount
                                   : CurrentPage + MaxDisplayedPages/2;
        }

        #region View

        public int StartDisplayedPage { get; protected set; }
        public int EndDisplayedPage { get; protected set; }

        public virtual string GetLinkForPage(int pageIndex)
        {
            return _queryBuilder.GetQueryStringWithParameter(ParameterName,
                                                             pageIndex.ToString(CultureInfo.InvariantCulture));
        }

        public virtual string GetLink()
        {
            return _queryBuilder.ToString();
        }

        #endregion
    }
}