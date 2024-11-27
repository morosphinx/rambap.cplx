using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Concepts;


public class InstanceCost : IInstanceConceptProperty
{
    public record NativeCostInfo (string name, Cost value);

    public List<NativeCostInfo> NativeCosts { get; init; } = new();
    public decimal Native {  get; init; }
    public decimal Composed { get; init; }
    public decimal Total => Native + Composed;
}

internal class CostsConcept : IConcept<InstanceCost>
{
    public override InstanceCost? Make(Pinstance instance, Part template)
    {
        // Calculate total native cost
        List<InstanceCost.NativeCostInfo> nativeCosts = new();
        ScanObjectContentFor<Cost>(template,
            (c, i) => nativeCosts.Add(new(i.Name,c)),
            AutoContent.IgnoreNulls);
        decimal totalnativeCost = nativeCosts.Sum(c => c.value.price);

        return new InstanceCost()
        {
            NativeCosts = nativeCosts,
            Native = totalnativeCost,
            Composed = instance.Components.Select(c => c.Instance.Cost()?.Total ?? 0).Sum()
        };
    }
}

