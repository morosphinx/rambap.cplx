using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using static rambap.cplx.Core.Support;
using static rambap.cplx.Modules.Costing.InstanceCost;

namespace rambap.cplx.Modules.Costing;


public class InstanceCost : IInstanceConceptProperty
{
    public abstract record CostPoint
    {
        public abstract Cost Value { get; }
        public abstract string Name { get; }
    }
    public sealed record NativeCost(Cost value, string name) : CostPoint
    {
        public override Cost Value { get; } = value;
        public override string Name { get; } = name;
    }
    public sealed record OfferCost(SupplierOffer offer) : CostPoint
    {
        public override Cost Value => Offer.Price.UnitPrice;
        public override string Name => Offer.Supplier.Company.Name;
        public SupplierOffer Offer { get; } = offer;
    }
    public sealed record SubcomponentCosts(Cost value) : CostPoint
    {
        public override Cost Value { get; } = value;
        public override string Name => "Subcomponents";
    }

    public required ReadOnlyCollection<NativeCost> NativeCosts { get; init; }

    /// <summary>
    /// List of offers to this part.<br/>
    /// Mutable, as offers may be added after instantiation through <see cref="Modules.SupplyChain.WorldModel.Quotation"/>
    /// </summary>
    public required List<SupplierOffer> AvailableOffers { get; init; }

    public IEnumerable<CostPoint> AllCostPoints(bool includeSubComponents = false)
    {
        if(selectedOffer != null)
            yield return new OfferCost(selectedOffer);
        foreach(var n in NativeCosts)
            yield return n;
        if(includeSubComponents && SubcomponentCostSum > 0)
            yield return new SubcomponentCosts(SubcomponentCostSum);
    }

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

    public decimal NativeCostSum => NativeCosts.Sum(c => c.value.Price);
    public required decimal SubcomponentCostSum { get; init; }

    /// <summary>
    /// Return the total cost of this PInstance, that is, the selected supplier offer,
    /// plus its other Costs and the Total Cost of its components
    /// </summary>
    public decimal TotalCost =>  NativeCostSum + SubcomponentCostSum + SupplierPrice().Price;
}

internal class CostsConcept : IConcept<InstanceCost>
{
    public override InstanceCost? Make(Component component)
    {
        var template = component.Template;
        // Calculate total native cost
        List<InstanceCost.NativeCost> nativeCosts = [];
        ScanObjectContentFor<Cost>(template,
            (c, i) => nativeCosts.Add(new(c,i.Name))
            );

        List<(string propName, Offer offer)> offers = [];
        ScanObjectContentFor<Offer>(template, (c, i) => offers.Add((i.Name, c)));

        List<SupplierOffer> supplierOffers = offers.Select(
            o => new SupplierOffer()
            {
                Supplier = o.offer.Supplier ?? o.propName,
                SKU = o.offer.SKU ?? component.PN,
                Link = o.offer.Link,
                Price = o.offer.Price,
            }).ToList();

        var subcomponents = component.SubComponents;
        bool anyComponentHasACost = subcomponents.Where(c => c.Instance.Cost() != null).Any();
        bool hasNativeCosts = nativeCosts.Count() > 0;
        bool hasSupplierOffer = offers.Count() > 0;
        bool hasACost = anyComponentHasACost || hasNativeCosts || hasSupplierOffer;
        if (!hasACost) return null; // Do not add a cost property needlessly

        decimal composedCost = subcomponents.Select(c => c.Instance.Cost()?.TotalCost ?? 0).Sum();
        return new InstanceCost()
        {
            NativeCosts = nativeCosts.AsReadOnly(),
            AvailableOffers = supplierOffers,
            SelectedOffer = supplierOffers.FirstOrDefault(),
            SubcomponentCostSum = composedCost
        };
    }
}

