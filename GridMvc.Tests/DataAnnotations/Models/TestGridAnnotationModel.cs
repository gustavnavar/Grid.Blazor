using System.ComponentModel.DataAnnotations;
using GridShared.DataAnnotations;
using GridMvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GridMvc.Tests.DataAnnotations.Models
{

    [ModelMetadataType(typeof(TestGridAnnotationMetadata))]
    internal class TestGridAnnotationModel
    {
        [GridColumn]
        public string Name { get; set; }


        public int Count { get; set; }

        [NotMappedColumn]
        public string NotMapped { get; set; }

        public string Title { get; set; }
    }

    [GridTable(PagingEnabled = true, PageSize = 20)]
    internal class TestGridAnnotationMetadata
    {
        [Display(Name = "Some title")]
        public string Title { get; set; }
    }
}
