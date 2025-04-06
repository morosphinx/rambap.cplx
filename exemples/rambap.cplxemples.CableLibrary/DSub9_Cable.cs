using rambap.cplx.Modules.Connectivity.Templates;
using System.Collections;

namespace rambap.cplxexemples.CableLibrary;

public class DSub9_Cable : Part
{
    HARTING_T1 C01 { get; set; }

    Cost AssemblyCost = 550;
}

// https://www.harting.com/fr-BE/telecharger-des-documents#query=catalog Connectivity catalog Page 534
// https://www.harting.com/en-US/p/D-SUB-9P-MALEDOUBLE-END-L-0-5M-33662145000058
// Many unproduced / not stocked versions
// https://www.harting.com/en-US/c/Device-connectivity-57981/Cable-connectors-and-cable-assemblies-58111/Power-and-signal-connectors-581670/list#query=:code-asc:inProductListAllowed:true:251:Cable%252Bassemblies:282:D-Sub
// https://www.mouser.fr/c/wire-cable/cable-assemblies/d-sub-cables/

public class DSub_Pin : Pin { }
public class DSub_9_M : Connector<DSub_Pin>
{
    public DSub_9_M() : base(9) {}

    public WireablePort Shell;
}

public class DSub_Socket : Pin { }
public class DSub_9_F : Connector<DSub_Socket>
{
    public DSub_9_F() : base(9) { }

    public WireablePort Shell;
}

[CplxHideContents]
public abstract class DSub_9_MF_Cable : Part, IPartConnectable
{
    DSub_9_M C01;
    DSub_9_F C02;

    ConnectablePort M => C01.MateFace;
    ConnectablePort F => C02.MateFace;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        foreach (var i in Enumerable.Range(1, C01.PinCount))
            Do.Wire(C01.Pin(i), C02.Pin(i));
        // TBD : Shielding ?
        Do.Wire(C01.Shell, C02.Shell);
    }
}

public class HARTING_T1 : DSub_9_MF_Cable
{
    SupplierOffer a = new()
    {
        Supplier = "RS",
        SupplierPN = "153-546-TBD",
        Price = 45
    };

    SupplierOffer b = new()
    {
        Supplier = "Farnell",
        SupplierPN = "153-546-TBD",
        Price = 412,
        Amount = 10,
    };
}

