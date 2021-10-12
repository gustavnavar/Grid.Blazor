using GridCore.DataAnnotations;
using GridShared.DataAnnotations;
using GridShared.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace GridMvc.DataAnnotations
{
    internal class GridAnnotationsProvider : GridCoreAnnotationsProvider
    {
        public override GridTableAttribute GetAnnotationForTable<T>()
        {
            var modelType = typeof(T).GetAttribute<ModelMetadataTypeAttribute>();
            if (modelType != null)
            {
                var metadataAttr = modelType.MetadataType.GetAttribute<GridTableAttribute>();
                if (metadataAttr != null)
                    return metadataAttr;
            }
            return typeof(T).GetAttribute<GridTableAttribute>();
        }

        public override PropertyInfo GetMetadataProperty<T>(PropertyInfo pi)
        {
            var modelType = typeof(T).GetAttribute<ModelMetadataTypeAttribute>();
            if (modelType != null)
            {
                PropertyInfo metadataProperty = modelType.MetadataType.GetProperty(pi.Name);
                if (metadataProperty != null)
                    return metadataProperty; //replace property
            }
            return pi;
        }
    }
}