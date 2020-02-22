using System;
using System.Collections.Generic;

namespace GridMvc.Demo
{
    public class SampleComparer : IComparer<string>
    {
        private readonly StringComparer _stringComparer;

        public SampleComparer(StringComparer stringComparer) => _stringComparer = stringComparer;

        public int Compare(string str1, string str2)
        {
            if (str1 == str2) return 0;
            if (str1 == null) return -1;
            if (str2 == null) return 1;
            if (str1.Length == 0) return -1;
            if (str2.Length == 0) return 1;

            var s1 = str1.Substring(1);
            var s2 = str2.Substring(1);

            return _stringComparer.Compare(s1, s2);
        }
    }
}
