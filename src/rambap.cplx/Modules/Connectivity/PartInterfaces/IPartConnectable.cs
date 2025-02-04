using rambap.cplx.Core;
using rambap.cplx.PartProperties;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartInterfaces;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Interface implemented by <see cref="Part"/>s that contains electrical wiring and cabling. <br/>
/// Define first a number of <see cref="ConnectablePort"/> on the Part and its component, <br/>
/// and define connection between those in the <see cref="Assembly_Connections"/> method
/// </summary>
public interface IPartConnectable
{
    /// <summary>
    /// Define the exposition of <see cref="ConnectablePort"/>s of the Part
    /// </summary>
    /// <param name="Do">A <see cref="PortBuilder"/> with the method difining expositions</param>
    public void Assembly_Ports(PortBuilder Do) { } // Implementation is optional

    /// <summary>
    /// Define the connection and of <see cref="ConnectablePort"/>s of the Part
    /// </summary>
    /// <param name="Do">A <see cref="ConnectionBuilder"/> with the method defining connection and wirings</param>
    public void Assembly_Connections(ConnectionBuilder Do);
}

