using rambap.cplx.Core;
using rambap.cplx.Attributes;
using rambap.cplx.PartInterfaces;
using rambap.cplx.PartProperties;
using System.Collections.ObjectModel;

namespace rambap.cplx.Modules.Connectivity.Templates;

public abstract class Connector : Part, IPartConnectable, ISingleMateable
{
    // ISingleMateablePart contract implementation
    [CplxIgnore]
    public ConnectablePort SingleMateablePort => MateFace;

    /// Implicit conversion to a ConnectablePort => the single mateFace
    public static implicit operator ConnectablePort(Connector c) => c.SingleMateablePort;

    public ConnectablePort MateFace ;

    ReadOnlyCollection<Pin> PinParts { get; init; }
    ReadOnlyCollection<WireablePort> WireablePorts { get; set; }

#pragma warning disable CS8618
    protected Connector(IEnumerable<(string,Pin)> pinAndNames)
    {
        PinParts = pinAndNames.Select(t => t.Item2).ToList().AsReadOnly();
        WireablePorts = pinAndNames.Select(t => new WireablePort() { IsPublic = true, Name = t.Item1 }).ToList().AsReadOnly();
        /// Mateface is constructed by cplx, and defined in <see cref="Assembly_Ports"/>
    }
#pragma warning restore CS8618


    // This is not public, because everyone is used tp identify pins with a 1-based indexed value
    // Force this to identify with a 1-based index through the Pin() method
    public int PinCount => WireablePorts.Count;

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
        return WireablePorts[oneBasedIndex - 1];
    }

    public void Assembly_Ports(PortBuilder Do)
    {
        // Expose MateFace, grouping of all pins
        var contacts = PinParts.Select(p => p.Contact);
        Do.ExposeAs(contacts, MateFace);
        // Expose Pins backs
        int i = 0;
        foreach (var pin in PinParts)
        {
            Do.ExposeAs(pin.Receptacle, WireablePorts[i++]);
        }
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {

    }
}

public abstract class Connector<T> : Connector, IPartConnectable, ISingleMateable
    where T : Pin, new()
{
    public Connector(int pinCount)
        : this(Enumerable.Range(0, pinCount).Select(i => $"{i + 1}").ToList())
    {
    }

    public Connector(List<string> pinNames) :
        base(pinNames.Select(s => (s, (Pin) new T())))
    {
    }
}