using System;
using System.Runtime.Serialization;

namespace GridBlazorGrpc.Shared.Models
{
    [DataContract]
    public class Response
    {
        [DataMember(Order = 1)]
        public bool Ok { get; set; }
        [DataMember(Order = 2)]
        public string Code { get; set; }
        [DataMember(Order = 3)]
        public string Message { get; set; }
        [DataMember(Order = 4)]
        public int Id { get; set; }

        public Response()
        { }

        public Response(bool res, string code = "", string message = "")
        {
            Ok = res;
            Code = code;
            Message = message;
        }

        public Response(bool res, int id, string code = "", string message = "")
            : this(res, code, message)
        {
            Id = id;
        }

        public Response(Exception exception, string code = "")
        {
            Ok = false;
            Code = code;
            Message = ((null == exception.InnerException) ? exception.Message : exception.Message + ":<br> " + exception.InnerException.Message).Replace('{', '(').Replace('}', ')');
        }
    }
}
