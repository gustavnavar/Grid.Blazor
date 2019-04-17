namespace GridMvc.Columns
{
    public interface ISGridColumn
    {
        IGridColumnHeaderRenderer HeaderRenderer { get; set; }
        IGridCellRenderer CellRenderer { get; set; }
    }
}
