using Newtonsoft.Json;

namespace GridShared.Utility
{
    public static class JsonHelper
    {
        /// <summary>
        ///     JSON Serialization
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /// <summary>
        ///     JSON Deserialization
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}