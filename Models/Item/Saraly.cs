namespace Dashboard.Models.Item;

class Saraly(string Month, bool Deduction, string PaymentItem, Money Money)
{
    public string Month { get; private set; } = Month;
    public bool Deduction { get; private set; } = Deduction;
    public string PaymentItem { get; private set; } = PaymentItem;
    public Money Money { get; private set; } = Money;
}
