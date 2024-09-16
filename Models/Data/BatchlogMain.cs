using System.Runtime.Serialization;

namespace Dashboard.Models.Data;

[DataContract]
public class BatchlogMain(string uuid, string status, string programId, string programName, string startTime, string endTime, List<BatchlogDetail> details) {
    [DataMember]
    public string Uuid { get; private set; } = uuid;
    [DataMember]
    public string Status { get; private set; } = status;
    [DataMember]
    public string ProgramId { get; private set; } = programId;
    [DataMember]
    public string ProgramName { get; private set; } = programName;
    [DataMember]
    public string StartTime { get; private set; } = startTime;
    [DataMember]
    public string EndTime { get; private set; } = endTime;
    [DataMember]
    public List<BatchlogDetail> Details { get; set; } = details;
}
