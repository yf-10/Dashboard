using System.Runtime.Serialization;

namespace Dashboard.Models.Data;

[DataContract]
public class Money(int amount, string? currencyCode) {
    [DataMember]
    public int Amount { get; private set; } = amount;
    [DataMember]
    public string CurrencyCode { get; private set; } = currencyCode ?? "JPY";
}