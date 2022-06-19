using GridShared.Utility;
using System.Runtime.Serialization;

namespace GridBlazorGrpc.Shared.Models
{
    [DataContract]
    public class Request
    { 
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 2)]
        public string Name { get; set; }
        [DataMember(Order = 3)]
        public QueryDictionary<string> Query { get; set; }

        public Request()
        { }

        public Request(int id, QueryDictionary<string> query = null)
        {
            Id = id;
            Query = query;
        }

        public Request(string name, QueryDictionary<string> query = null)
        {
            Name = name;
            Query = query;
        }

        public Request(int id, string name, QueryDictionary<string> query = null)
        {
            Id = id;
            Name = name;
            Query = query;
        }
    }
}
