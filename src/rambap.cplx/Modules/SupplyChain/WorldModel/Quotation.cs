using rambap.cplx.Core;
using rambap.cplx.Modules.Costing;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.SupplyChain.WorldModel;

public abstract class QuotationItems
{
    internal abstract void TryApplyTo(Component component, Supplier supplier);
}

public class QuotationItem<P> : QuotationItems
    where P : Part
{

    /// <summary>
    /// Supplier stock keeping unit, if null or empty, the part PN is assumed to be the SKU
    /// </summary>
    public string? SKU { get; init; }
    public string? Link { get; init; }
    public required PriceTag Price { get; init; }

    private bool CanApplyTo(Component component)
    {
        // TODO : this will break if Part Type is not an identity, such as the part having been instantiated
        // with a parametered constructor
        return component.Instance.PartType == typeof(P); 
    }
    internal override void TryApplyTo(Component component, Supplier supplier)
    {
        if (CanApplyTo(component))
        {
            component.Instance.Cost()?.AvailableOffers.Add(
                new SupplierOffer
                {
                    Supplier = supplier,
                    SKU = SKU ?? component.PN,
                    Link = Link,
                    Price = Price,
                });
        }
    }
}

public class Quotation
{
    public required Supplier Supplier { get; init; }

    public required List<QuotationItems> Items { get; init; } = new() ;

    public void ApplyTo(Component component)
    {
        foreach(var item in Items)
        {
            item.TryApplyTo(component, Supplier);
        }
        foreach(var subcomponent in component.SubComponents)
        {
            this.ApplyTo(subcomponent);
        }
    }
}
