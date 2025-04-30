using rambap.cplx.Modules.SupplyChain.WorldModel;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Costing;

/// <summary>
/// Realised Supplier Offer in a Pinstance
/// </summary>
public record SupplierOffer
{
    public required Supplier Supplier { get; init; }
    public required string SKU { get; init; }
    public string? Link { get; init; }

    public required PriceTag Price { get; init; }
}
