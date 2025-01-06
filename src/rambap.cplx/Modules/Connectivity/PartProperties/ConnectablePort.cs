using rambap.cplx.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Define a connectable element, generaly electrical, on a Part. <br/>
/// To be then used by Parts implementing <see cref="PartInterfaces.IPartConnectable"/> <br/>
/// Define <see cref="ConnectablePort"/> as public when they can be seen and used from outside the Part.
/// </summary>
public class ConnectablePort : SignalPort
{
 
}

