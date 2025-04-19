using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.SupplyChain.WorldModel;

public abstract class QuotationItems
{
    internal abstract void TryApplyTo(Component component, Supplier supplier, TimeSpan deliveryDelay);
}

public class QuotationItem<P> : QuotationItems
    where P : Part
{

    /// <summary>
    /// Supplier stock keeping unit, if null or empty, the part PN is assumed to be the SKU
    /// </summary>
    public string? SupplierSKU { get; init; }
    public uint Amount { get; init; } = 1;
    public required Cost Cost { get; init; }
    public Cost UnitPrice => Cost.Price / Amount;

    public string? Link { get; init; }


    private bool CanApplyTo(Component component)
    {
        // TODO : this will break if Part Type is not an identity, such as the part having been instantiated
        // with a parametered constructor
        return component.Instance.PartType == typeof(P); 
    }
    internal override void TryApplyTo(Component component, Supplier supplier, TimeSpan deliveryDelay)
    {
        if (CanApplyTo(component))
        {
            component.Instance.Cost()?.AvailableOffers.Add(
                new SupplierOffer
                {
                    Supplier = supplier,
                    Amount = Amount,
                    Cost = Cost,
                    SupplierSKU = SupplierSKU,
                    DeliveryDelay = deliveryDelay,
                    Link = Link,
                });
        }
    }
}

public class Quotation
{
    public required Supplier Supplier { get; init; }
    public TimeSpan DeliveryDelay { get; init; }


    public required List<QuotationItems> Items { get; init; } = new() ;

    public void ApplyTo(Component component)
    {
        foreach(var item in Items)
        {
            item.TryApplyTo(component, Supplier, DeliveryDelay);
        }
        foreach(var subcomponent in component.SubComponents)
        {
            this.ApplyTo(subcomponent);
        }
    }
}
