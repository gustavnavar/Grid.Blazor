using System;

namespace GridShared.DataAnnotations
{
    /// <summary>
    ///     Marks property as hidden Grid.Mvc column
    /// </summary>
    public class GridHiddenColumnAttribute : Attribute
    {
        public GridHiddenColumnAttribute()
        {
            EncodeEnabled = true;
            SanitizeEnabled = true;
            Key = false;
        }

        /// <summary>
        ///     Specify that content of this column need to be encoded
        /// </summary>
        public bool EncodeEnabled { get; set; }

        /// <summary>
        ///     Specify that content of this column need to be sanitized
        /// </summary>
        public bool SanitizeEnabled { get; set; }

        /// <summary>
        ///     Specify the format of column data
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        ///     Specify if column data is key
        /// </summary>
        public bool Key { get; set; }

        /// <summary>
        ///     Sets or get column position
        /// </summary>
        public int Position { get; set; }
    }
}