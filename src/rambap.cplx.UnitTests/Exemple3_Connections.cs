namespace rambap.cplx.UnitTests;

class RackConnected1 : Part, IPartConnectable
{
    InternalCable1 Cable1, Cable2;

    public Connector J11, J12, J21, J22;

    public void Assembly_Connections(IPartConnectable.ConnectionBuilder Do)
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

    public Connector J01;
    public Connector J02;
    public Connector PWR_P;
    public Connector PWR_N;

    public void Assembly_Connections(IPartConnectable.ConnectionBuilder Do)
    {
        // D38 to D38 connection
        Do.Connect(C01.A, C02.B);
        Do.Connect(C01.B, C02.A);
        Do.Connect(C01.C, C02.C);
        Do.Connect(C01.D, C02.D);
        Do.Connect(C01.E, C02.E);
        Do.Connect(C01.F, C02.F);
        Do.Connect(C01.G, C02.G);
        // Power
        Do.Connect(C01.J, C04.SolderPoint);
        Do.Connect(C01.K, C05.SolderPoint);
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
    public Connector Face; // D38999 Face

    public Connector A, B, C, D, E, F, G, H, I, J, K;

    Cost Buy = 200;
}

class Test4mm : Part
{
    public Connector SolderPoint;
    public Connector TestPlug4mmMale;

    Cost Buy = 10;
}

