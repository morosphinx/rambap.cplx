namespace rambap.cplx.UnitTests;
using rambap.cplx.Modules.Connectivity.Model;

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
    D38999_1Connector C01;
    D38999_1Connector C02;
    Test4mm C04;
    Test4mm C05;

    public ConnectablePort J01;
    public ConnectablePort J02;
    public ConnectablePort PWR_P;
    public ConnectablePort PWR_N;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // D38 to D38 connection
        Do.Wire(C01.A, C02.B);
        Do.Wire(C01.B, C02.A);
        Do.Wire(C01.C, C02.C);
        Do.Wire(C01.D, C02.D);
        Do.Wire(C01.E, C02.E);
        Do.Wire(C01.F, C02.F);
        Do.Wire(C01.G, C02.G);
        // Power
        Do.Wire(C01.J, C04.SolderPoint);
        Do.Wire(C01.K, C05.SolderPoint);
        // Exposed connection
        Do.ExposeAs(C01.Face, J01);
        Do.ExposeAs(C02.Face, J02);
        Do.ExposeAs(C04.TestPlug4mmMale, PWR_P);
        Do.ExposeAs(C05.TestPlug4mmMale, PWR_N);
    }

    Cost Assembly = 200;
    Cost SmallParts = 200;
}

class D38999_1Connector : Part
{
    public ConnectablePort Face; // D38999 Face

    public WireablePort A, B, C, D, E, F, G, H, I, J, K;

    Cost Buy = 200;
}

class Test4mm : Part
{
    public WireablePort SolderPoint;
    public ConnectablePort TestPlug4mmMale;

    Cost Buy = 10;
}

