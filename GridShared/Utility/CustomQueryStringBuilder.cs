using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;


namespace GridShared.Utility
{
    /// <summary>
    ///     Class retain the current query string parameters
    /// </summary>
    public class CustomQueryStringBuilder : NameValueCollection
    {
        public static NameValueCollection Convert(IQueryCollection collection)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            foreach (var element in collection)
            {
                nameValueCollection.Set(element.Key, element.Value);
            }
            return nameValueCollection;
        }    

        public static NameValueCollection Convert(IQueryDictionary<StringValues> collection)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            foreach (var element in collection)
            {
                nameValueCollection.Set(element.Key, element.Value);
            }
            return nameValueCollection;
        }

        public CustomQueryStringBuilder(IQueryDictionary<StringValues> collection)
            : base(Convert(collection))
        {
        }

        public CustomQueryStringBuilder(IQueryCollection collection)
            : base(Convert(collection))
        {
        }

        public override string ToString()
        {
            return GetQueryStringExcept(new string[0]);
        }

        public string GetQueryStringWithParameter(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            if (base[parameterName] != null)
                base[parameterName] = parameterValue;
            else
                base.Add(parameterName, parameterValue);

            return ToString();
        }

        /// <summary>
        ///     Returns query string without parameter values
        /// </summary>
        /// <param name="parameterNames">Parameter values</param>
        public string GetQueryStringExcept(IList<string> parameterNames)
        {
            var result = new StringBuilder();
            foreach (string key in base.AllKeys)
            {
                if (string.IsNullOrEmpty(key) || parameterNames.Contains(key))
                    continue;
                string[] values = base.GetValues(key);
                if (values != null && values.Count() != 0)
                {
                    if (result.Length == 0)
                        result.Append("?");
                    foreach (string value in values)
                    {
                        // added to support multiple filters
                        string[] vals = value.Split(',');
                        foreach (string val in vals)
                        {
                            result.Append(key + "=" + WebUtility.UrlEncode(val) + "&");
                        }
                    }
                }
            }
            string resultString = result.ToString();
            return resultString.EndsWith("&") ? resultString.Substring(0, resultString.Length - 1) : resultString;
        }
    }
}