using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Modules.Costing;


public class InstanceCost : IInstanceConceptProperty
{
    public record NativeCostInfo(string name, Cost value);

    public required ReadOnlyCollection<NativeCostInfo> NativeCosts { get; init; }
    public required ReadOnlyCollection<SupplierOffer> AvailableOffers { get; init; }
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
    public Cost SupplierCost()
    {
        if (AvailableOffers.Count() == 0) return 0; // Part may not use supplier, it's ok to not have any offer defined
        else if (selectedOffer == null)
            throw new InvalidOperationException($"Offers are avaiable for this part but none has been selected");
        else return SelectedOffer!.UnitPrice;
    }

    public required decimal Native { get; init; }
    public required decimal Composed { get; init; }
    public decimal Total => Native + Composed + SupplierCost().Price;
}

internal class CostsConcept : IConcept<InstanceCost>
{
    public override InstanceCost? Make(Pinstance instance, Part template)
    {
        // Calculate total native cost
        List<InstanceCost.NativeCostInfo> nativeCosts = [];
        ScanObjectContentFor<Cost>(template,
            (c, i) => nativeCosts.Add(new(i.Name, c))
            );

        List<SupplierOffer> offers = [];
        ScanObjectContentFor<SupplierOffer>(template, (c, i) => offers.Add(c));

        bool anyComponentHasACost = instance.Components.Where(c => c.Instance.Cost() != null).Any();
        bool hasNativeCosts = nativeCosts.Count() > 0;
        bool hasSupplierOffer = offers.Count() > 0;
        bool hasACost = anyComponentHasACost || hasNativeCosts || hasSupplierOffer;
        if (!hasACost) return null; // Do not add a cost property needlessly

        decimal totalnativeCost = nativeCosts.Sum(c => c.value.Price);
        decimal composedCost = instance.Components.Select(c => c.Instance.Cost()?.Total ?? 0).Sum();
        return new InstanceCost()
        {
            NativeCosts = nativeCosts.AsReadOnly(),
            AvailableOffers = offers.AsReadOnly(),
            SelectedOffer = offers.FirstOrDefault(),
            Native = totalnativeCost,
            Composed = composedCost
        };
    }
}

