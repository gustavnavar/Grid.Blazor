using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GridShared.Totals
{
    /// <summary>
    ///     Pager DTO
    /// </summary>
    [DataContract]
    public class TotalsDTO
    {
        [DataMember(Order = 1)]
        public IDictionary<string,string> Sum { get; set; }
        [DataMember(Order = 2)]
        public IDictionary<string, string> Average { get; set; }
        [DataMember(Order = 3)]
        public IDictionary<string, string> Max { get; set; }
        [DataMember(Order = 4)]
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