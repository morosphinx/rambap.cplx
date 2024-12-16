namespace rambap.cplx.PartProperties;

/// <summary>
/// An amount of money that need to be spend to acquire or produce the part
/// </summary>
/// <param name="Price">Amount</param>
/// <param name="Currency">Currency. If unspecified, use a default currency<br/>
/// Currency are compared by string equality
/// </param>
public record Cost(decimal Price, string Currency = "")
{
    public static implicit operator Cost(decimal price) => new (price);
    public static implicit operator Cost(double price) => new ((decimal) price);
    public static implicit operator Cost(int price) => new (price);
}