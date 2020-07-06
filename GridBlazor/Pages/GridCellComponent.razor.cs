using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;

namespace GridBlazor.Pages
{
    public partial class GridCellComponent<T>
    {
        private const string TdStyle = "display:none;";

        private int _sequence = 0;
        protected string _cssStyles;
        protected string _cssClass;
        protected MarkupString _cell;
        protected Type _componentType;
        protected RenderFragment _cellRender;

        public object ChildComponent { get; private set; } = null;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

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
            if (_componentType != null)
                _cellRender = CreateComponent(_sequence, GridComponent, _componentType, Column, Item, 
                    RowId, new VariableReference(ChildComponent));
            else
                _cell = (MarkupString)Column.GetCell(Item).ToString();
            if (Column.Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + TdClass;

            if (!string.IsNullOrWhiteSpace(Column.Width))
                _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();

            string columnCssClasses = Column.GetCellCssClasses(Item);
            if(!string.IsNullOrWhiteSpace(columnCssClasses))
                _cssClass += " " + columnCssClasses;
        }

        protected RenderFragment CreateComponent(int sequence, GridComponent<T> gridComponent, Type componentType, 
            IGridColumn column, object item, int rowId, VariableReference reference) => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", gridComponent);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", GridCellComponent<T>
                .CreateComponent(sequence, componentType, column, item, rowId, false, reference));
            builder.CloseComponent();
        };

        public static RenderFragment CreateComponent(int sequence, Type componentType, IGridColumn column,
            object item, int? RowId, bool crud, VariableReference reference)  => builder =>
        {
            if (componentType != null)
            {
                builder.OpenComponent(++sequence, componentType);
                builder.AddAttribute(++sequence, "Item", item);
                var gridProperty = componentType.GetProperty("Grid");
                if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                    builder.AddAttribute(++sequence, "Grid", (CGrid<T>)column.ParentGrid);
                gridProperty = componentType.GetProperty("Actions");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Actions", crud ? column.CrudActions : column.Actions);
                gridProperty = componentType.GetProperty("Functions");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Functions", crud ? column.CrudFunctions : column.Functions);
                gridProperty = componentType.GetProperty("Object");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Object", crud ? column.CrudObject : column.Object);
                gridProperty = componentType.GetProperty("RowId");
                if (gridProperty != null && RowId.HasValue)
                    builder.AddAttribute(++sequence, "RowId", RowId.Value);
                builder.AddComponentReferenceCapture(++sequence, r => reference.Variable = r);
                builder.CloseComponent();
            }           
        };
    }
}
