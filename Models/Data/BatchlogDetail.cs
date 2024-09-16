using System.Runtime.Serialization;

namespace Dashboard.Models.Data;

[DataContract]
public class BatchlogDetail(string? uuid, int? logNo, string? logMsg, string? logTime) {
    [DataMember]
    public string? Uuid { get; private set; } = uuid;
    [DataMember]
    public int? LogNo { get; private set; } = logNo;
    [DataMember]
    public string? LogMsg { get; private set; } = logMsg;
    [DataMember]
    public string? LogTime { get; private set; } = logTime;
}
