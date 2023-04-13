using GridShared.DataAnnotations;
using GridShared.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

    [GridTable(PagingType = PagingType.Pagination, PageSize = 20)]
    internal class TestGridAnnotationMetadata
    {
        [Display(Name = "Some title")]
        public string Title { get; set; }
    }
}
