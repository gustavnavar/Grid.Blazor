using GridBlazor.Columns;
using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;

namespace GridBlazor
{
    public class GridCellComponentBase<T> : ComponentBase
    {
        private const string TdClass = "grid-cell";
        private const string TdStyle = "display:none;";

        protected string _cssStyles;
        protected string _cssClass;

        [Parameter]
        protected IGridColumn Column { get; set; }
        [Parameter]
        protected IGridCell Cell { get; set; }

        protected override void OnParametersSet()
        {
            if (((GridColumnBase<T>)Column).Hidden)
                _cssStyles = ((GridStyled)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyled)Column).GetCssStylesString();
            _cssClass = ((GridStyled)Column).GetCssClassesString() + " " + TdClass;
        }
    }
}