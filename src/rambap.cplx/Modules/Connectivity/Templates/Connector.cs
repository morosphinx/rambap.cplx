using rambap.cplx.Core;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;

namespace rambap.cplx.Modules.Connectivity.Templates;

public abstract class Connector<T> : Part, IPartConnectable
    where T : Pin, new()
{
    public int PinCount { get; init; }
    List<T> PinParts { get; }

    public ConnectablePort MateFace { get; }
    public ReadOnlyCollection<WireablePort> Pins { get; }

    public Connector(int pinCount)
    {
        PinCount = pinCount;
        var pinParts = Enumerable.Range(1, PinCount).Select(i => new T());
        PinParts = pinParts.ToList();
        MateFace = new();
        Pins = pinParts.Select(p => new WireablePort()).ToList().AsReadOnly();
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // Expose MateFace
        var contacts = PinParts.Select(p => p.Contact);
        Do.ExposeAs(contacts,MateFace);
        // Expose Pins backs
        foreach(var i in Enumerable.Range(0, PinCount))
        {
            Do.ExposeAs(PinParts[i].Receptacle, Pins[i]);
        }
    }
}
