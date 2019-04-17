using System;

namespace GridShared.DataAnnotations
{
    /// <summary>
    ///     Marks property as not a column. Grid.Mvc will not add this property to the column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotMappedColumnAttribute : Attribute
    {
    }
}