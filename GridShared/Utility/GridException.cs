using System;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class GridException : Exception
    {
        public string Code { get; set; }
        public IEnumerable<string> Attributes { get; set; }

        public GridException(string message) :
            this(null, null, message, null)
        {
        }

        public GridException(Exception innerException) :
            this(null, null, GetInnerMessage(innerException), innerException)
        {
        }

        public GridException(string code, string message) :
            this(code, null, message, null)
        {
        }

        public GridException(string code, Exception innerException) :
            this(code, null, GetInnerMessage(innerException), innerException)
        {
        }

        public GridException(string code, IEnumerable<string> attributes, string message) : 
            this(code, attributes, message, null)
        {
        }

        public GridException(string code, IEnumerable<string> attributes, Exception innerException) :
            this(code, attributes, GetInnerMessage(innerException), innerException)
        {
        }

        public GridException(string code, IEnumerable<string> attributes, string message, Exception innerException) 
            : base(message.Replace('{', '(').Replace('}', ')'), innerException)
        {
            Code = code;
            Attributes = attributes;
        }

        public static string GetInnerMessage(Exception e)
        {
            if (e == null)
                return null;
            else if (e.InnerException != null)
                return GetInnerMessage(e.InnerException);
            else
                return e.Message;
        }
    }
}
