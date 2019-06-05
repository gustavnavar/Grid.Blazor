namespace GridBlazor.Columns
{
    public interface ICGridColumn
    {
        bool IsSumEnabled { get; }

        string SumString { get; set; }

        bool IsAverageEnabled { get; }

        string AverageString { get; set; }

        bool IsMaxEnabled { get; }

        string MaxString { get; set; }

        bool IsMinEnabled { get; }

        string MinString { get; set; }
    }
}
