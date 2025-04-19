using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Modules.Mass;

public class InstanceMass : IInstanceConceptProperty
{
    public record NativeMassInfo(string name, Mass_kg value);

    public List<NativeMassInfo> NativeMasses { get; init; } = [];
    public required Mass_kg Native { get; init; }
    public required Mass_kg Composed { get; init; }
    public Mass_kg Total => Native + Composed;
}

internal class MassConcept : IConcept<InstanceMass>
{
    public override InstanceMass? Make(Pinstance instance, IEnumerable<Component> subcomponents, Part template)
    {
        // Calculate total native mass
        List<InstanceMass.NativeMassInfo> nativeMasses = [];
        ScanObjectContentFor<Mass_kg>(template,
            (c, i) => nativeMasses.Add(new(i.Name, c))
            );
        decimal totalnativeMass = nativeMasses.Sum(c => c.value.mass_kg);

        return new InstanceMass()
        {
            NativeMasses = nativeMasses,
            Native = totalnativeMass,
            Composed = subcomponents.Select(c => c.Instance.Mass()?.Total ?? 0)
                        .Select(m => m.mass_kg).Sum()
        };
    }
}


