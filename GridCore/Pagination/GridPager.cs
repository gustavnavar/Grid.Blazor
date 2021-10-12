using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;

namespace GridCore.Pagination
{
    /// <summary>
    ///     Default grid pager implementation
    /// </summary>
    public class GridPager : IGridPager
    {
        public const int DefaultMaxDisplayedPages = 5;
        public const int DefaultPageSize = 20;

        public const string DefaultPageQueryParameter = "grid-page";
        public const string DefaultPageSizeQueryParameter = "grid-pagesize";
        public const string DefaultPagerViewName = "_GridPager";
        public const string DefaultAjaxPagerViewName = "_AjaxGridPager";

        private readonly IQueryDictionary<StringValues> _query;
        private readonly CustomQueryStringBuilder _queryBuilder;
        private int _currentPage;

        private int _itemsCount;
        private int _maxDisplayedPages;
        private int _pageSize;
        private int _queryPageSize;

        #region ctor's

        public GridPager(IQueryDictionary<StringValues> query)
        {
            if (query == null)
                throw new Exception("No http context here!");

            _query = query;
            _currentPage = -1;
            _queryBuilder = new CustomQueryStringBuilder(query);

            ParameterName = DefaultPageQueryParameter;
            TemplateName = DefaultPagerViewName;
            MaxDisplayedPages = MaxDisplayedPages;

            string pageSizeParameter = query.Get(DefaultPageSizeQueryParameter);
            int pageSize = 0;
            if (pageSizeParameter != null)
                int.TryParse(pageSizeParameter, out pageSize);
            QueryPageSize = pageSize;

            PageSize = DefaultPageSize;
        }

        #endregion

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
        
        public bool ChangePageSize { get; set; }

        public int QueryPageSize
        {
            get { return _queryPageSize; }
            set
            {
                _queryPageSize = value;
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
            if (_queryPageSize != 0)
                _pageSize = _queryPageSize;
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
        public string TemplateName { get; set; }

        public virtual string GetLinkForPage(int pageIndex)
        {
            return _queryBuilder.GetQueryStringWithParameter(ParameterName,
                                                             pageIndex.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}