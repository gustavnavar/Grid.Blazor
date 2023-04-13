using GridShared.Pagination;
using System;

namespace GridShared.DataAnnotations
{
    /// <summary>
    ///     Specify common grid.mvc options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GridTableAttribute : Attribute
    {
        public GridTableAttribute()
        {
            PagingType = PagingType.None;
            PageSize = 0;
            PagingMaxDisplayedPages = 0;
        }

        /// <summary>
        ///     Enable or disable paging of the grid
        /// </summary>
        public PagingType PagingType { get; set; }

        [Obsolete("This property is obsolete. Use PagingType property", true)]
        /// <summary>
        ///     Enable or disable paging of the grid
        /// </summary>
        public bool PagingEnabled { get; set; }

        /// <summary>
        ///     Sets ot get page size of the grid
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Sets ot get count of displaying pages
        /// </summary>
        public int PagingMaxDisplayedPages { get; set; }
    }
}