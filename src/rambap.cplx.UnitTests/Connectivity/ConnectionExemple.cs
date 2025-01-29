using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.UnitTests.Connectivity;


class Box0 : Part, IPartConnectable
{
    BoxAA AA;
    BoxB B;
    CableC C;

    public BoxA A => AA.A;

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

class CableC2 : Part, IPartConnectable
{
    public ConnectablePort L;
    public ConnectablePort R;

    ConnectorC2 U1;
    ConnectorC2 U2;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(U1.MateFace, L);
        Do.ExposeAs(U2.MateFace, R);
    }
}
class ConnectorC2: Part
{
    public ConnectablePort MateFace;
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
    public BoxE E;
    public BoxB B;
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
        Do.Wire(C01.Pin(1), C02.Pin(1));
        Do.Wire(C01.Pin(2), C02.Pin(2));
        Do.Wire(C01.Pin(3), C02.Pin(3));
        Do.Wire(C01.Pin(4), C02.Pin(4));
        // 2 to another connector, mixed
        Do.Wire(C01.Pin(6), C03.Pin(1));
        Do.Wire(C01.Pin(7), C03.Pin(2));
        // A multiconnector distribution
        Do.Wire(C01.Pin(10), C02.Pin(10));
        Do.Wire(C01.Pin(10), C03.Pin(10));
        // A loopback
        Do.Wire(C03.Pin(7), C03.Pin(8));

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

class BoxAAA : Part, IPartConnectable
{
    WiringA P1;
    WiringA P2;

    CableC2 c2;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.Mate(P1.J01, P2.J01);
        Do.CableWith(c2, P1.J02, P2.J02);
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
    [TestMethod]
    public void TestConnectionWiringAAA()
    {
        var p = new BoxAAA();
        CablingConnectionsTests.WriteConnection(ConnectorIdentity.Topmost, p);
    }
}