namespace rambap.cplx.PartProperties;

/// <summary>
/// An amount of money that need to be spend to acquire or produce the part
/// </summary>
/// <param name="price">Amount</param>
/// <param name="currency">Currency. If unspecified, use a default currency<br/>
/// Currency are compared by string equality
/// </param>
public record Cost(decimal price, string currency = "")
{
    public static implicit operator Cost(decimal price) => new Cost(price);
    public static implicit operator Cost(double price) => new Cost((decimal) price);
    public static implicit operator Cost(int price) => new Cost((decimal) price);
}