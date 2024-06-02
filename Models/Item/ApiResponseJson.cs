using System.Runtime.Serialization;

namespace Dashboard.Models.Item;

[DataContract]
class ApiResponseJson(int statuscode, string message, dynamic? data)
{
    [DataMember]
    public int statuscode { get; private set; } = statuscode;
    [DataMember]
    public string message { get; private set; } = message;
    [DataMember]
    public dynamic? data { get; private set; } = data;
}
