namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            TValue val;
            source.TryGetValue(key, out val);
            return val;
        }

        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (value == null)
                throw new ArgumentException("key");

            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }
    }
}
