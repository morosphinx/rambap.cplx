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

    /// <summary>
    /// Link to the supplier website or catalog
    /// </summary>
    public string? Link { get; init; }

    /// <summary>
    /// Price and conditions for this offer
    /// </summary>
    public required PriceTag Price { get; init; }

    public Offer() { }

    public static implicit operator Offer(decimal price) => new() { Price = price };
    public static implicit operator Offer(double price) => new() { Price = price };
    public static implicit operator Offer(int price) => new() { Price = price };
}

/// <summary>
/// A cost and conditions for an <see cref="Offer"/> <br/>
/// Include packing details and delay<br/>
/// Do not add this property on a Part. add instead an <see cref="Offer"/>
/// </summary>
public sealed record PriceTag
{
    /// <summary>
    /// Cost to pay for the delivery of one unit, <br/> <b>or</b>
    /// Cost for a pack of this unit if <see cref="PackUnitCount"/> is not 1
    /// </summary>
    public required Cost Cost { get; init; }

    /// <summary>
    /// If > 1, this price tag refer to a pack of unit
    /// </summary>
    public uint PackUnitCount { get; init; } = 1;

    /// <summary>
    /// Ideal cost of a single unit <br/>
    /// </summary>
    /// <remarks>
    /// This does not takes into account additional unit that need to be brought due to either :<br/>
    /// - Packing : <see cref="PackUnitCount"/><br/>
    /// - Minimum order quantities <see cref="PackMOQ"/>
    /// </remarks>
    public Cost UnitPrice => Cost.Price / PackUnitCount;

    /// <summary>
    /// Minimum order quantity, per pack
    /// </summary>
    public uint PackMOQ { get; init; } = 1;

    /// <summary>
    /// Delivery delay for the complete buy order
    /// </summary>
    public TimeSpan? DeliveryDelay { get; init; }

    public static implicit operator PriceTag(decimal price) => new() { Cost = price };
    public static implicit operator PriceTag(double price) => new() { Cost = price };
    public static implicit operator PriceTag(int price) => new() { Cost = price };
}