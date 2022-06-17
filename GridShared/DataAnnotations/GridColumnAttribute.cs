using GridShared.Columns;
using GridShared.Sorting;
using System;

namespace GridShared.DataAnnotations
{
    /// <summary>
    ///     Marks property as Grid.Mvc column, with specified parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GridColumnAttribute : GridHiddenColumnAttribute
    {
        private GridSortDirection? _initialDirection;

        private AutoCompleteTerm? _autoCompleteTaxonomy;

        public GridColumnAttribute()
        {
            EncodeEnabled = true;
            SanitizeEnabled = true;
            SortEnabled = false;
            Key = false;
        }

        /// <summary>
        ///     Sets or get the column title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Enable or disable column sorting
        /// </summary>
        public bool SortEnabled { get; set; }

        /// <summary>
        ///     Enable or disable column filtering
        /// </summary>
        public bool FilterEnabled { get; set; }

        /// <summary>
        ///     Sets or get column width,
        ///     Sample: "100px", "13%" ...
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        ///     Sets or get column custom filter widget type
        /// </summary>
        public string FilterWidgetType { get; set; }


        /// <summary>
        ///     Sets or get sort initial direction
        /// </summary>
        public GridSortDirection SortInitialDirection
        {
            get { return _initialDirection.HasValue ? _initialDirection.Value : GridSortDirection.Ascending; }
            set { _initialDirection = value; }
        }

        public AutoCompleteTerm AutocompleteTaxonomy
        {
            get { return _autoCompleteTaxonomy.HasValue ? _autoCompleteTaxonomy.Value : AutoCompleteTerm.None; }
            set { _autoCompleteTaxonomy = value; }
        }

        public GridSortDirection? GetInitialSortDirection()
        {
            return _initialDirection;
        }

        public AutoCompleteTerm? GetAutocompleteTaxonomy()
        {
            return _autoCompleteTaxonomy;
        }

    }
}