using System.Runtime.Serialization;

namespace Dashboard.Models.Item;

[DataContract]
public class SalariesJson(List<Salary> salaries)
{
    [DataMember]
    public List<Salary> Salaries { get; private set; } = salaries;
}
