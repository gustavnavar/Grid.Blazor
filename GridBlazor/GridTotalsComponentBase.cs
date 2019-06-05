using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridTotalsComponentBase<T> : ComponentBase
    {
        [Parameter]
        protected ICGrid<T> Grid { get; set; }
    }
}
