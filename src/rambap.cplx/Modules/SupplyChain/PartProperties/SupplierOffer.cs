#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public record SupplierOffer
{
    public required Supplier Supplier { get; init; }

    /// <summary>
    /// Supplier PN, if null or empty, the part PN is assumed to be the manufacturer PN
    /// </summary>
    public string? SupplierPN { get; init; }

    public uint Amount { get; init; } = 1;

    public required Cost Price { get; init; }

    public string? Link { get; init; }

    public Cost UnitPrice => Price.Price / Amount;
}
