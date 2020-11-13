using GridShared.Columns;
using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridColComponent
    {
        protected string _cssStyles;

        [Parameter]
        public IGridColumn Column { get; set; }

        protected override void OnParametersSet()
        {
            const string HiddenStyle = "display:none;";

            if (Column.Hidden)
            {
                _cssStyles = HiddenStyle;
            }
            else
            {
                _cssStyles = "";
            }

            if (!string.IsNullOrWhiteSpace(Column.Width))
            {
                _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();
            }
        }
    }
}
