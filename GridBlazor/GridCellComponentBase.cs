using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;

namespace GridBlazor
{
    public class GridCellComponentBase : ComponentBase
    {
        private const string TdClass = "grid-cell";
        private const string TdStyle = "";

        protected string _cssStyles;
        protected string _cssClass;

        [Parameter]
        protected IGridColumn Column { get; set; }
        [Parameter]
        protected IGridCell Cell { get; set; }

        protected override void OnParametersSet()
        {
            _cssStyles = ((GridStyled)Column).GetCssStylesString() + " " + TdStyle;
            _cssClass = ((GridStyled)Column).GetCssClassesString() + " " + TdClass;
        }
    }
}