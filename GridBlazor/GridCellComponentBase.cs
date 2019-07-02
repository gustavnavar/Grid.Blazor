using GridBlazor.Columns;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;

namespace GridBlazor
{
    public class GridCellComponentBase<T> : ComponentBase
    {
        private const string TdClass = "grid-cell";
        private const string TdStyle = "display:none;";

        private int _sequence = 0;
        protected string _cssStyles;
        protected string _cssClass;
        protected MarkupString _cell;
        protected Type _componentType;
        protected RenderFragment _cellRender;

        [Parameter]
        protected IGridColumn Column { get; set; }
        [Parameter]
        protected object Item { get; set; }

        protected override void OnParametersSet()
        {
            _componentType = ((GridColumnBase<T>)Column).ComponentType;
            if (_componentType != null)
                _cellRender = CreateCellComponent();
            else
                _cell = (MarkupString)Column.GetCell(Item).ToString();
            if (((GridColumnBase<T>)Column).Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + TdClass;
            string columnCssClasses = Column.GetCellCssClasses(Item);
            if(!string.IsNullOrWhiteSpace(columnCssClasses))
                _cssClass += " " + columnCssClasses;
        }

        private RenderFragment CreateCellComponent() => builder =>
        {
            builder.OpenComponent(++_sequence, _componentType);
            builder.AddAttribute(++_sequence, "Item", Item);
            builder.CloseComponent();
        };
    }
}