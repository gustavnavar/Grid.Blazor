using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;

namespace GridBlazor.Pages
{
    public partial class GridCellComponent<T>
    {
        private const string TdStyle = "display:none;";

        protected string _cssStyles;
        protected string _cssClass;
        protected MarkupString _cell;
        protected Type _componentType;
        protected RenderFragment _cellRender;

        public object ChildComponent { get; private set; } = null;

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public IGridColumn Column { get; set; }

        [Parameter]
        public object Item { get; set; }

        [Parameter]
        public int RowId { get; set; }

        [Parameter]
        public string TdClass { get; set; } = "grid-cell";

        protected override void OnParametersSet()
        {
            _componentType = Column.ComponentType;
            _cell = default;
            _cellRender = default;
            if (_componentType != null)
                _cellRender = CreateComponent(GridComponent, _componentType, Column, Item, 
                    RowId, false, new VariableReference(ChildComponent));
            else
                _cell = (MarkupString)Column.GetCell(Item).ToString();
            if (Column.Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + TdClass;

            if (GridComponent.Grid.Direction == GridDirection.RTL)
                _cssStyles = string.Concat(_cssStyles, " text-align:right;direction:rtl;").Trim();

            if (!string.IsNullOrWhiteSpace(Column.Width))
                _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();

            string columnCssClasses = Column.GetCellCssClasses(Item);
            if(!string.IsNullOrWhiteSpace(columnCssClasses))
                _cssClass += " " + columnCssClasses;
        }

        public static RenderFragment CreateComponent(GridComponent<T> gridComponent, Type componentType, 
            IGridColumn column, object item, int? rowId, bool crud, VariableReference reference) => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", gridComponent);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", CreateComponent(componentType, column, item, rowId, crud, reference));
            builder.CloseComponent();
        };

        private static RenderFragment CreateComponent(Type componentType, IGridColumn column,
            object item, int? RowId, bool crud, VariableReference reference)  => builder =>
        {
            if (componentType != null)
            {
                builder.OpenComponent(0, componentType);
                builder.AddAttribute(1, "Item", item);
                var gridProperty = componentType.GetProperty("Grid");
                if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                    builder.AddAttribute(2, "Grid", (CGrid<T>)column.ParentGrid);
                gridProperty = componentType.GetProperty("Actions");
                if (gridProperty != null)
                    builder.AddAttribute(3, "Actions", crud ? column.CrudActions : column.Actions);
                gridProperty = componentType.GetProperty("Functions");
                if (gridProperty != null)
                    builder.AddAttribute(4, "Functions", crud ? column.CrudFunctions : column.Functions);
                gridProperty = componentType.GetProperty("Object");
                if (gridProperty != null)
                    builder.AddAttribute(5, "Object", crud ? column.CrudObject : column.Object);
                gridProperty = componentType.GetProperty("RowId");
                if (gridProperty != null && RowId.HasValue)
                    builder.AddAttribute(6, "RowId", RowId.Value);
                builder.AddComponentReferenceCapture(7, r => reference.Variable = r);
                builder.CloseComponent();
            }           
        };
    }
}
