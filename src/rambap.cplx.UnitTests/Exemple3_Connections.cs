namespace rambap.cplx.UnitTests;
using rambap.cplx.Modules.Connectivity.Model;
using rambap.cplx.Modules.Connectivity.Templates;

class RackConnected1 : Part, IPartConnectable
{
    InternalCable1 Cable1, Cable2;

    public ConnectablePort J11, J12, J21, J22;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(Cable1.J01, J11);
        Do.ExposeAs(Cable1.J01, J12);
        Do.ExposeAs(Cable2.J01, J21);
        Do.ExposeAs(Cable2.J01, J22);
    }

    Cost Rack = 500;
    Cost Visserie = 50;
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

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // D38 to D38 connection
        Do.Wire(C01.A, C02.Pins[0]);
        Do.Wire(C01.B, C02.Pins[1]);
        Do.Wire(C01.C, C02.Pins[2]);
        Do.Wire(C01.D, C02.Pins[3]);
        Do.Wire(C01.E, C02.Pins[4]);
        Do.Wire(C01.F, C02.Pins[5]);
        Do.Wire(C01.G, C02.Pins[6]);
        // Power
        Do.Wire(C01.J, C04.SolderPoint);
        Do.Wire(C01.K, C05.SolderPoint);
        // Exposed connection
        Do.ExposeAs(C01.Face, J01);
        Do.ExposeAs(C02.MateFace, J02);
        Do.ExposeAs(C04.TestPlug4mmMale, PWR_P);
        Do.ExposeAs(C05.TestPlug4mmMale, PWR_N);
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

    public void Assembly_Connections(ConnectionBuilder Do)
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
        ],Face);
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

    public WireablePort A => Pins[0];
    public WireablePort B => Pins[1];
    public WireablePort C => Pins[2];
    public WireablePort D => Pins[3];
    public WireablePort E => Pins[4];
    public WireablePort F => Pins[5];
    public WireablePort G => Pins[6];
    public WireablePort H => Pins[7];
    public WireablePort I => Pins[8];
    public WireablePort J => Pins[9];
    public WireablePort K => Pins[10];
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

