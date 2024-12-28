using rambap.cplx.Core;
using rambap.cplx.PartProperties;

namespace rambap.cplx.PartInterfaces;

/// <summary>
/// Interface implemented by <see cref="Part"/>s that contains electrical wiring and cabling. <br/>
/// Define first a number of <see cref="Connector"/> on the Part and its component, <br/>
/// and define connection between those in the <see cref="Assembly_Connections"/> method
/// </summary>
public partial interface IPartConnectable
{
    /// <summary>
    /// Define the connection and exposition of <see cref="Connector"/>s of the Part
    /// </summary>
    /// <param name="Do">A <see cref="ConnectionBuilder"/> with the method difining wiirng</param>
    public void Assembly_Connections(ConnectionBuilder Do);
}

