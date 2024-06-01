namespace Dashboard.Models.Item;

class Money(int Amount, string? CurrencyCode)
{
    public int Amount { get; private set; } = Amount;
    public string CurrencyCode { get; private set; } = CurrencyCode ?? "JPY";
}