using rambap.cplx.Attributes;
using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace rambap.cplx.Modules.Connectivity.Templates;

/// <summary>
/// Define a Wire spool, from witch <see cref="WirePart"/> can be cut.
/// </summary>
public abstract class WireSpool : Part
{
    internal WirePart GetWirePart(double length)
    {
        return new WirePart()
        {
            Origin = this,
            Length = length,
        };
    }

    /// <summary>
    /// Length, in cm, of a unit of thsi wire spool when ordered
    /// </summary>
    public required double UnitPackingLength { get; init; } = 3000;

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

/// <summary>
/// A single length of wire, carrying a single electric signal <br/>
/// This is implicitly created when calling <see cref="ConnectionBuilder.Wire"/> in a <see cref="IPartConnectable"/>
/// </summary>
public class WirePart : Part, IPartConnectable
{
    /// <summary>
    /// The spool this wire was taken from. Define the wire.
    /// </summary>
    [CplxIgnore]
    public required WireSpool Origin { get; init; } // Not a subcomponent

    /// <summary>
    /// Length, in cm, of the wire
    /// </summary>
    public required double Length { get; init; }

    // Set during cplx part initialisation
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WireEnd LeftPort;
    public WireEnd RightPort;
    internal WirePart() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.StructuralWire(LeftPort, RightPort, Origin);
    }
}