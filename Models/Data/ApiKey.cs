using System.Runtime.Serialization;

namespace Dashboard.Models.Data;

[DataContract]
public class ApiKey(User user, string key, string status)
{
    [DataMember]
    public User User { get; private set; } = user;
    [DataMember]
    public string Key { get; private set; } = key;
    [DataMember]
    public string Status { get; private set; } = status;
}
