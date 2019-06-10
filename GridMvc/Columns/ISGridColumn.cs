namespace GridMvc.Columns
{
    public interface ISGridColumn
    {
        bool IsSumEnabled { get; }

        decimal? SumValue { get; set; }

        string SumString { get; set; }

        bool IsAverageEnabled { get; }

        decimal? AverageValue { get; set; }

        string AverageString { get; set; }

        bool IsMaxEnabled { get; }

        object MaxValue { get; set; }

        string MaxString { get; set; }

        bool IsMinEnabled { get; }

        object MinValue { get; set; }

        string MinString { get; set; }

        string ValuePattern { get; }
    }
}
