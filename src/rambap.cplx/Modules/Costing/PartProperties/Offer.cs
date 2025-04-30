using rambap.cplx.Modules.SupplyChain.WorldModel;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// On a Part, declare a supplier offer, such as a quotation or web catalog price. <br/>
/// </summary>
/// 
/// <remarks>
/// Remark : <br/>
/// Placing this information on a <see cref="Core.Part"/> library may not be advisable, because : <br/>
///     - Pricing or availability may change frequently <br/>
///     - Pricing and avaibility depend on location (national, etc) <br/>
///     - Differential pricing, due to custom quotations or web algorithms, exist<br/>
///     - Up to date data is often gated behind APIs with constraining term &amp; condition<br/>
/// <br/>
/// As a consequence, place <see cref="Offer"/> on a <see cref="Core.Part"/> only if the Part
/// is inside the immediate cplx source<br/>
/// (eg : executed assembly or a project in same VisualStudio solution)<br/>
/// <br/>
/// Quotation and supplier offer data for library components can be added after the Part Instantiation <br/>
/// using <see cref="rambap.cplx.Modules.SupplyChain.WorldModel.Quotation"/>s
/// </remarks>
public record class Offer
{
    /// <summary>
    /// Supplier identification. If not provided, the property name is assumed to be the supplier
    /// </summary>
    public Supplier? Supplier { get; init; }

    /// <summary>
    /// Supplier stock keeping unit. If not provided, the part PN is assumed to be the SKU
    /// </summary>
    public string? SKU { get; init; }

    public string? Link { get; init; }

    public required PriceTag Price { get; init; }

    // Various part syntax attemps, for now

    // Does not set required member, need to specify the Price
    public Offer() { }

    public static implicit operator Offer(decimal price) => new() { Price = price };
    public static implicit operator Offer(double price) => new() { Price = price };
    public static implicit operator Offer(int price) => new() { Price = price };
}

public sealed record PriceTag
{
    public uint Amount { get; init; } = 1;
    public required Cost Cost { get; init; }
    public TimeSpan? DeliveryDelay { get; init; }
    public Cost UnitPrice => Cost.Price / Amount;

    public static implicit operator PriceTag(decimal price) => new() { Cost = price };
    public static implicit operator PriceTag(double price) => new() { Cost = price };
    public static implicit operator PriceTag(int price) => new() { Cost = price };
}