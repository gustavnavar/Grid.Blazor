using System.Collections.Generic;

namespace GridShared.Totals
{
    /// <summary>
    ///     Pager DTO
    /// </summary>
    public class TotalsDTO
    {
        public IDictionary<string,string> Sum { get; set; }
        public IDictionary<string, string> Average { get; set; }
        public IDictionary<string, string> Max { get; set; }
        public IDictionary<string, string> Min { get; set; }

        public TotalsDTO()
        {
            Sum = new Dictionary<string, string>();
            Average = new Dictionary<string, string>();
            Max = new Dictionary<string, string>();
            Min = new Dictionary<string, string>();
        }

        public TotalsDTO(IDictionary<string, string> sum, IDictionary<string, string> average,
            IDictionary<string, string> max, IDictionary<string, string> min)
        {
            Sum = sum;
            Average = average;
            Max = max;
            Min = min;
        }
    }
}