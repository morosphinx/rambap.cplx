using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Modules.Costing;


public class InstanceCost : IInstanceConceptProperty
{
    public record NativeCostInfo(string name, Cost value);

    public required ReadOnlyCollection<NativeCostInfo> NativeCosts { get; init; }

    /// <summary>
    /// List of offers to this part.<br/>
    /// Mutable, as offers may be added after instantiation through <see cref="Modules.SupplyChain.WorldModel.Quotation"/>
    /// </summary>
    public required List<SupplierOffer> AvailableOffers { get; init; }

    private SupplierOffer? selectedOffer;
    public SupplierOffer? SelectedOffer
    {
        get => selectedOffer;
        set
        {
            if(value == null) selectedOffer = value; // Allow setting null value
            else if (!AvailableOffers.Contains(value))
                throw new InvalidOperationException($"The offer {value} is not from this part");
            else selectedOffer = value;
        }
    }
    public Cost SupplierPrice()
    {
        if (AvailableOffers.Count() == 0) return 0; // Part may not use supplier, it's ok to not have any offer defined
        else if (selectedOffer == null)
            throw new InvalidOperationException($"Offers are avaiable for this part but none has been selected");
        else return SelectedOffer!.Price.UnitPrice;
    }

    public required decimal Native { get; init; }
    public required decimal Composed { get; init; }

    public static bool SupplierCostIsReplacement { get; set; } = true;
    public decimal Total =>
        AvailableOffers.Count() > 0 && SupplierCostIsReplacement // There ar offer, and we are using offers only
        ? SupplierPrice().Price // Only the offer Price
        : Native + Composed + SupplierPrice().Price;
}

internal class CostsConcept : IConcept<InstanceCost>
{
    public override InstanceCost? Make(Pinstance instance, IEnumerable<Component> subcomponents, Part template)
    {
        // Calculate total native cost
        List<InstanceCost.NativeCostInfo> nativeCosts = [];
        ScanObjectContentFor<Cost>(template,
            (c, i) => nativeCosts.Add(new(i.Name, c))
            );

        List<(string propName, Offer offer)> offers = [];
        ScanObjectContentFor<Offer>(template, (c, i) => offers.Add((i.Name, c)));

        List<SupplierOffer> supplierOffers = offers.Select(
            o => new SupplierOffer()
            {
                Supplier = o.offer.Supplier ?? o.propName,
                SKU = o.offer.SKU ?? instance.PN,
                Link = o.offer.Link,
                Price = o.offer.Price,
            }).ToList();

        bool anyComponentHasACost = subcomponents.Where(c => c.Instance.Cost() != null).Any();
        bool hasNativeCosts = nativeCosts.Count() > 0;
        bool hasSupplierOffer = offers.Count() > 0;
        bool hasACost = anyComponentHasACost || hasNativeCosts || hasSupplierOffer;
        if (!hasACost) return null; // Do not add a cost property needlessly

        decimal totalnativeCost = nativeCosts.Sum(c => c.value.Price);
        decimal composedCost = subcomponents.Select(c => c.Instance.Cost()?.Total ?? 0).Sum();
        return new InstanceCost()
        {
            NativeCosts = nativeCosts.AsReadOnly(),
            AvailableOffers = supplierOffers,
            SelectedOffer = supplierOffers.FirstOrDefault(),
            Native = totalnativeCost,
            Composed = composedCost
        };
    }
}

