using rambap.cplx.Attributes;
using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace rambap.cplx.Modules.Connectivity.Templates;

public abstract class WireSpool : Part
{
    // To add to a part

    internal WirePart GetWirePart(double length)
    {
        return new WirePart()
        {
            Origin = this,
            Length = length,
        };
    }

    public required double UnitPackingLength { get; init; } = 30;

    public required Color Color { get; init; } = Color.Gray;

    public required WireDiameter Diameter { get; init; } = WireDiameter.AWG24;

    public enum WireDiameter
    {
        AWG28,
        AWG26,
        AWG24,
        AWG22,
        AWG20,
        AWG18,
        AWG16,
        AWG14,
        AWG12,
        AWG10,
        AWG8,
        AWG6,
        AWG4,
        AWG2,
        AWG0,
    }
}

[PartDescription("Wire definition auto created by cplx. Used by default on wiring action unless othermise specified.")]
public class PlaceholderWireSpool : WireSpool
{
    [SetsRequiredMembers]
    public PlaceholderWireSpool() { }
}

public class WirePart : Part
{
    [CplxIgnore]
    public required WireSpool Origin { get; init; } // Not a subcomponent

    public required double Length { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WireEnd LeftPort;
    public WireEnd RightPort;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}