namespace rambap.cplxtests.UsageTests;

using DocumentFormat.OpenXml.Packaging;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Modules.Connectivity.Templates;

class RackConnected1 : Part, IPartConnectable, IPartAdditionalDocuments
{
    InternalCable1 Cable1, Cable2;

    public ConnectablePort J11, J12, J21, J22;

    public void Assembly_Ports(PortBuilder Do)
    {
        Do.ExposeAs(Cable1.J01, J11);
        Do.ExposeAs(Cable1.J02, J12);
        Do.ExposeAs(Cable2.J01, J21);
        Do.ExposeAs(Cable2.J02, J22);
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
    }

    public void Additional_Documentation(DocumentationBuilder Do)
    {
        Do.AddInstruction("CustomFile.txt", new CustomDoc());
    }

    Cost Rack = 500;
    Cost Visserie = 50;
}

class CustomDoc : TextCustomFile
{
    public override string GetText()
    {
        return "Test of a custom documentation content defined in the part itself";
    }
}

class InternalCable1 : Part, IPartConnectable
{
    ComplexeConnectorDeclarationKind1 C01;
    ComplexConnectorDeclarationKind2 C02;
    Test4mm C04;
    Test4mm C05;

    public ConnectablePort J01;
    public ConnectablePort J02;
    public ConnectablePort PWR_P;
    public ConnectablePort PWR_N;

    public void Assembly_Ports(PortBuilder Do)
    {
        // Exposed connection
        Do.ExposeAs(C01.Face, J01);
        Do.ExposeAs(C02.MateFace, J02);
        Do.ExposeAs(C04.TestPlug4mmMale, PWR_P);
        Do.ExposeAs(C05.TestPlug4mmMale, PWR_N);
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // D38 to D38 connection
        Do.Wire(C01.A, C02.Pin(1));
        Do.Wire(C01.B, C02.Pin(2));
        Do.Wire(C01.C, C02.Pin(3));
        Do.Wire(C01.D, C02.Pin(4));
        Do.Wire(C01.E, C02.Pin(5));
        Do.Wire(C01.F, C02.Pin(6));
        Do.Wire(C01.G, C02.Pin(7));
        // Power
        Do.Wire(C01.J, C04.SolderPoint);
        Do.Wire(C01.K, C05.SolderPoint);
    }

    Cost Assembly = 200;
    Cost SmallParts = 200;
}

class ComplexeConnectorDeclarationKind1 : Part, IPartConnectable
{
    // Won't scale. But WireablePortscan be addigned by name

    public ConnectablePort Face; // D38999 Face

    public WireablePort A, B, C, D, E, F, G, H, I, J, K;

    Size24pin pA, pB, pC, pD, pE, pF, pG, pH, pI, pJ, pK;
    public void Assembly_Ports(PortBuilder Do)
    {
        // Expose mating face
        Do.ExposeAs(
        [
            pA.Contact,
            pB.Contact,
            pC.Contact,
            pD.Contact,
            pE.Contact,
            pF.Contact,
            pG.Contact,
            pH.Contact,
            pI.Contact,
            pJ.Contact,
            pK.Contact,
        ], Face);
        // Expose wireable pins
        Do.ExposeAs(pA.Receptacle, A);
        Do.ExposeAs(pB.Receptacle, B);
        Do.ExposeAs(pC.Receptacle, C);
        Do.ExposeAs(pD.Receptacle, D);
        Do.ExposeAs(pE.Receptacle, E);
        Do.ExposeAs(pF.Receptacle, F);
        Do.ExposeAs(pG.Receptacle, G);
        Do.ExposeAs(pH.Receptacle, H);
        Do.ExposeAs(pI.Receptacle, I);
        Do.ExposeAs(pJ.Receptacle, J);
        Do.ExposeAs(pK.Receptacle, K);
    }
    public void Assembly_Connections(ConnectionBuilder Do)
    {

    }
}

class ComplexConnectorDeclarationKind2 : Connector<Size24pin>
{
    // Simple, but the pins must be accesed by index (0-indexed) in the parent
    public ComplexConnectorDeclarationKind2() : base(11)
    { }
}

class ComplexConnectorDeclarationKind3 : Connector<Size24pin>
{
    // Same as kind2, but add explicit pin naming

    public WireablePort A => Pin(1);
    public WireablePort B => Pin(2);
    public WireablePort C => Pin(3);
    public WireablePort D => Pin(4);
    public WireablePort E => Pin(5);
    public WireablePort F => Pin(6);
    public WireablePort G => Pin(7);
    public WireablePort H => Pin(8);
    public WireablePort I => Pin(9);
    public WireablePort J => Pin(10);
    public WireablePort K => Pin(11);
    public ComplexConnectorDeclarationKind3() : base(11)
    { }
}

class Size24pin : Pin {}

class Test4mm : Part
{
    public WireablePort SolderPoint;
    public ConnectablePort TestPlug4mmMale;

    Cost Buy = 10;
}

