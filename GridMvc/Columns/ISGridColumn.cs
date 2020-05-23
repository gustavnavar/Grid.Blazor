namespace GridMvc.Columns
{
    public interface ISGridColumn
    {
        decimal? SumValue { get; set; }

        decimal? AverageValue { get; set; }

        object MaxValue { get; set; }

        object MinValue { get; set; }

        string ValuePattern { get; }
    }
}
