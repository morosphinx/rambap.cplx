using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Concepts.Costing;


public class InstanceCost : IInstanceConceptProperty
{
    public record NativeCostInfo(string name, Cost value);

    public List<NativeCostInfo> NativeCosts { get; init; } = new();
    public required decimal Native { get; init; }
    public required decimal Composed { get; init; }
    public decimal Total => Native + Composed;
}

internal class CostsConcept : IConcept<InstanceCost>
{
    public override InstanceCost? Make(Pinstance instance, Part template)
    {
        // Calculate total native cost
        List<InstanceCost.NativeCostInfo> nativeCosts = new();
        ScanObjectContentFor<Cost>(template,
            (c, i) => nativeCosts.Add(new(i.Name, c)),
            AutoContent.IgnoreNulls);

        bool anyComponentHasACost = instance.Components.Where(c => c.Instance.Cost() != null).Any();
        bool hasACost = anyComponentHasACost || nativeCosts.Any();
        if (!hasACost) return null; // Do not add a cost property needlessly

        decimal totalnativeCost = nativeCosts.Sum(c => c.value.price);
        decimal composedCost = instance.Components.Select(c => c.Instance.Cost()?.Total ?? 0).Sum();
        return new InstanceCost()
        {
            NativeCosts = nativeCosts,
            Native = totalnativeCost,
            Composed = composedCost
        };
    }
}

