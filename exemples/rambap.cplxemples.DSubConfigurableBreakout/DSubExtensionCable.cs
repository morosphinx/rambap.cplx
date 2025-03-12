using rambap.cplxexemples.CableLibrary;
namespace rambap.cplxexemples.DSubConfigurableBreakout; 

public class DSubExtensionCable : Part, IPartConnectable
{
    Harting_DSub_0967_CrimpAssembly CO_A;
    Harting_DSub_0967_CrimpAssembly CO_B;

    Wire Wire;

    public ConnectablePort J01 => CO_A.MateFace;
    public ConnectablePort J02 => CO_B.MateFace;

    public DSubExtensionCable(DIN41652_PinCounts pinCount)
    {
        PN = $"MyCable{pinCount}";
        CO_A = new Harting_DSub_0967_CrimpAssembly(DIN41652_Genders.Male, pinCount);
        CO_B = new Harting_DSub_0967_CrimpAssembly(DIN41652_Genders.Female, pinCount);
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        foreach(int i in Enumerable.Range(1,CO_A.PinCount))
            Do.Wire(CO_A.Pin(i),CO_B.Pin(i));
    }
}

public class Wire : Part
{

}

public class CableBundle : Part
{
    cplxexemples.CableLibrary.MyPart mypart;
    DSubExtensionCable W01 = new DSubExtensionCable(DIN41652_PinCounts._09);
    DSubExtensionCable W02 = new DSubExtensionCable(DIN41652_PinCounts._15);
}