using System.Runtime.Serialization;

namespace Dashboard.Models.Data;

[DataContract]
public class ApiResponseJson(int status, string message, int dataCount, dynamic? data) {
    [DataMember]
    public int Status { get; private set; } = status;
    [DataMember]
    public string Message { get; private set; } = message;
    [DataMember]
    public int DataCount { get; private set; } = dataCount;
    [DataMember]
    public dynamic? Data { get; private set; } = data;

}
