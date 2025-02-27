using rambap.cplx.Core;
using rambap.cplx.Attributes;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;

namespace rambap.cplx.Modules.Connectivity.Templates;

public abstract class Connector<T> : Part, IPartConnectable, ISingleMateable
    where T : Pin, new()
{
    public int PinCount { get; init; }
    List<T> PinParts { get; }

    public ConnectablePort MateFace { get; }

    // ISingleMateablePart contract implementation
    [CplxIgnore]
    public ConnectablePort SingleMateablePort => MateFace;

    /// Implicit conversion to a ConnectablePort => the single mateFace
    public static implicit operator ConnectablePort(Connector<T> c) => c.SingleMateablePort;

    // This is not public, because everyone is used tp identify pins with a 1-based indexed value
    // Force this to identify with a 1-based index through the Pin() method
    ReadOnlyCollection<WireablePort> WireablePorts { get; }

    /// <summary>
    /// Return the wireable Port with the *_1-based_* pin index 
    /// </summary>
    /// <param name="oneIndex">1-based pin index.<br/>
    /// Eg : the first pin is GetPin(1), the last pint is Pin(PinCount)</param>
    /// <returns></returns>
    public WireablePort Pin(int oneBasedIndex)
    {
        if (PinCount == 0)
            throw new InvalidOperationException("There are no pins in this connector");
        if (oneBasedIndex == 0 || oneBasedIndex > PinCount)
            throw new InvalidOperationException($"Invalid index. Use 1-based index to access pins. The first pin is {nameof(Pin)}(1). The last pin is {nameof(Pin)}({PinCount})");
        return PinParts[oneBasedIndex - 1].Receptacle;
    }


    public Connector(int pinCount) 
        : this(Enumerable.Range(0,pinCount).Select(i => $"{i+1}").ToList())
    {
    }

    public Connector(List<string> pinNames)
    {
        PinCount = pinNames.Count();
        var pinParts = Enumerable.Range(1, PinCount).Select(i => new T());
        PinParts = pinParts.ToList();
        MateFace = new() { IsPublic = true };
        WireablePorts = pinParts.Select(p => new WireablePort() { IsPublic = true }).ToList().AsReadOnly();
        int idx = 0;
        foreach (var pin in WireablePorts)
            pin.Name = pinNames[idx++];
    }

    public void Assembly_Ports(PortBuilder Do)
    {
        // Expose MateFace
        var contacts = PinParts.Select(p => p.Contact);
        Do.ExposeAs(contacts, MateFace);
        // Expose Pins backs
        foreach (var i in Enumerable.Range(0, PinCount))
        {
            Do.ExposeAs(PinParts[i].Receptacle, WireablePorts[i]);
        }
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {

    }
}
