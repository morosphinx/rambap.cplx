namespace rambap.cplxexemple.DSubConfigurableBreakout; 

public class MyCable : Part, IPartConnectable
{
    Harting_DSub_CrimpAssembly CO_A;
    Harting_DSub_CrimpAssembly CO_B;

    Wire Wire;

    public Connector J01;
    public Connector J02;

    public MyCable(DIN41652_PinCounts pinCount)
    {
        PN = $"MyCable{pinCount}";
        CO_A = new Harting_DSub_CrimpAssembly(DIN41652_Genders.Male, pinCount);
        CO_B = new Harting_DSub_CrimpAssembly(DIN41652_Genders.Female, pinCount);
    }

    public void Assembly_Connections(IPartConnectable.ConnectionBuilder Do)
    {
        Do.ExposeAs(CO_A.MatingFace, J01);
        Do.ExposeAs(CO_B.MatingFace, J02);
    }
}

public class Wire : Part
{

}

public class CableBundle : Part
{
    MyCable W01 = new MyCable(DIN41652_PinCounts._09);
    MyCable W02 = new MyCable(DIN41652_PinCounts._15);
}