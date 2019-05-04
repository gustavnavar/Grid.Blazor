using System;

namespace GridShared.DataAnnotations
{
    /// <summary>
    /// This attribute specifies the metadata class to associate with a data model class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GridMetadataTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridMetadataTypeAttribute" /> class.
        /// </summary>
        /// <param name="type">The type of metadata class that is associated with a data model class.</param>
        public GridMetadataTypeAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            MetadataType = type;
        }

        /// <summary>
        /// Gets the type of metadata class that is associated with a data model class.
        /// </summary>
        public Type MetadataType { get; }
    }
}
