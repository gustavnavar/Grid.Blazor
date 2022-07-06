using GridShared.Utility;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GridShared.Totals
{
    /// <summary>
    ///     Totals DTO
    /// </summary>
    [DataContract]
    public class TotalsDTO
    {
        [DataMember(Order = 1)]
        public IDictionary<string, Total> Sum { get; set; }
        [DataMember(Order = 2)]
        public IDictionary<string, Total> Average { get; set; }
        [DataMember(Order = 3)]
        public IDictionary<string, Total> Max { get; set; }
        [DataMember(Order = 4)]
        public IDictionary<string, Total> Min { get; set; }
        [DataMember(Order = 5)]
        public IDictionary<string, QueryDictionary<Total>> Calculations { get; set; }

        public TotalsDTO()
        {
            Sum = new Dictionary<string, Total>();
            Average = new Dictionary<string, Total>();
            Max = new Dictionary<string, Total>();
            Min = new Dictionary<string, Total>();
            Calculations = new Dictionary<string, QueryDictionary<Total>>();
        }

        public TotalsDTO(IDictionary<string, Total> sum, IDictionary<string, Total> average, IDictionary<string, Total> max, 
            IDictionary<string, Total> min, IDictionary<string, QueryDictionary<Total>> calculations)
        {
            Sum = sum;
            Average = average;
            Max = max;
            Min = min;
            Calculations = calculations;
        }
    }
}