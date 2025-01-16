using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.UnitTests.Connectivity;


class Box0 : Part, IPartConnectable
{
    BoxAA AA;
    BoxB B;
    CableC C;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(C, AA.A.J01, B.J02);
    }
}

class BoxAA : Part
{
    public BoxA A;
}

class BoxA : Part
{
    public ConnectablePort J01;
}

class BoxB : Part
{
    public ConnectablePort J02;
}

class CableC : Part
{
    ConnectablePort L;
    ConnectablePort R;
}


class BoxD : Part
{
    public BoxA AL;
    public BoxA AR;
}

class BoxE : Part
{
    BoxD D;
    public BoxA AL => D.AL;
}
class BoxF : Part, IPartConnectable
{
    BoxE E;
    BoxB B;
    CableC C;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(C, E.AL.J01, B.J02);
    }
}

class BoxE2 : Part, IPartConnectable
{
    BoxD D;
    public ConnectablePort J03;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(D.AL.J01, J03);
    }
}
class BoxF2 : Part, IPartConnectable
{
    BoxE2 E;
    BoxB B;
    CableC C;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(C, E.J03, B.J02);
    }
}

// Invalid syntax
class BoxE3 : Part, IPartConnectable
{
    public BoxD D;
    BoxA SubA;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(SubA.J01, D.AL.J01);
    }
}
class BoxF3 : Part, IPartConnectable
{
    BoxE3 E;
    BoxB B;
    CableC C;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(C, E.D.AL.J01, B.J02);
    }
}

class WiringA : Part, IPartConnectable
{
    public ConnectablePort J01;
    public ConnectablePort J02;
    public ConnectablePort J03;

    ConnectorA C01;
    ConnectorA C02;
    ConnectorA C03;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        // 4 straitgh connection
        Do.Wire(C01.Pins[0], C02.Pins[0]);
        Do.Wire(C01.Pins[1], C02.Pins[1]);
        Do.Wire(C01.Pins[2], C02.Pins[2]);
        Do.Wire(C01.Pins[3], C02.Pins[3]);
        // 2 to another connector, mixed
        Do.Wire(C01.Pins[5], C03.Pins[0]);
        Do.Wire(C01.Pins[6], C03.Pins[1]);
        // A multiconnector distribution
        Do.Wire(C01.Pins[9], C02.Pins[9]);
        Do.Wire(C01.Pins[9], C03.Pins[9]);
        // A loopback
        Do.Wire(C03.Pins[6], C03.Pins[7]);

        // Expositions
        Do.ExposeAs(C01, J01);
        Do.ExposeAs(C02, J02);
        Do.ExposeAs(C03.MateFace, J03);
    }
}
class ConnectorA : Connector<PinA>
{
    public ConnectorA() : base(10){}
}
class PinA : Pin { }

class WiringB : Part, IPartConnectable
{
    ComplexConnectorDeclarationKind3 C01;
    ComplexConnectorDeclarationKind3 C02;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.Wire(C01.A, C02.B);
        Do.Wire(C02.A, C01.B);
    }
}


[TestClass]
public class TestSomeOutputs
{
    [TestMethod]
    public void TestConnectionExemple1()
    {
        var p = new Box0();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }

    [TestMethod]
    public void TestConnectionExempleE()
    {
        var p = new BoxF();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }

    [TestMethod]
    public void TestConnectionExempleE2()
    {
        var p = new BoxF2();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }
    [TestMethod]
    public void TestConnectionExempleE3()
    {
        var p = new BoxF3();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }
    [TestMethod]
    public void TestConnectionWiringA()
    {
        var p = new WiringA();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }
    [TestMethod]
    public void TestConnectionWiringB()
    {
        var p = new WiringB();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }
}