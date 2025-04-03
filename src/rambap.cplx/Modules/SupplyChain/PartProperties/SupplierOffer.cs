#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// On a Part, declare a supplier offer, such as a quotation or web catalog price. <br/>
/// </summary>
/// 
/// <remarks>
/// Placing this information on a <see cref="Core.Part"/> library may not be advisable, because : <br/>
///     - Pricing or availability may change frequently <br/>
///     - Pricing and avaibility depend on location (national, etc) <br/>
///     - Differential pricing, due to custom quotations or web algorithms, exist<br/>
///     - Up to date data is often gated behind APIs with constraining term &amp; condition<br/>
/// <br/>
/// As a consequence, use <see cref="SupplierOffer"/> on a <see cref="Core.Part"/> only if the Part
/// is inside the immediate cplx source<br/>
/// (eg : executed assembly or a project in same VisualStudio solution)<br/>
/// <br/>
/// Quotation and supplier offer data for library components can be added after the Part Instantiation <br/>
/// using <see cref="Modules.SupplyChain.WorldModel.Quotation"/>s
/// </remarks>
public record SupplierOffer
{
    public required Supplier Supplier { get; init; }

    /// <summary>
    /// Supplier stock keeping unit, if null or empty, the part PN is assumed to be the SKU
    /// </summary>
    public string? SupplierSKU { get; init; }
    public uint Amount { get; init; } = 1;
    public required Cost Cost { get; init; }
    public Cost UnitPrice => Cost.Price / Amount;

    public string? Link { get; init; }

    public TimeSpan DeliveryDelay { get; init; }
}
