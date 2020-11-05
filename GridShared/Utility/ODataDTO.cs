using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public class ODataDTO<T>
    {
        [JsonPropertyName("@odata.context")]
        public string Context { get; set; }
        [JsonPropertyName("@odata.count")]
        public int ItemsCount { get; set; }
        [JsonPropertyName("value")]
        public IEnumerable<T> Value { get; set; }

        public ODataDTO()
        {
        }

        public ODataDTO(IEnumerable<T> items)
        {
            Value = items;
        }
    }
}
