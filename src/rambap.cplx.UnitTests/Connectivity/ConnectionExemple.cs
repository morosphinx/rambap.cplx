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

class CableC2 : Part
{
    public ConnectablePort L => U1.MateFace;
    public ConnectablePort R => U2.MateFace;

    ConnectorC2 U1;
    ConnectorC2 U2;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
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
    public ConnectablePort J03 => D.AL.J01;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
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

    public void Assembly_Ports(PortBuilder Do)
    {
        Do.ExposeAs(SubA.J01, D.AL.J01);
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
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
    public ConnectablePort J01 => C01.MateFace;
    public ConnectablePort J02 => C02.MateFace;
    public ConnectablePort J03 => C03.MateFace;

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
    public void TestConnectionExemple1() => TestOutputs.WriteConnection(new Box0());

    [TestMethod]
    public void TestConnectionExempleE() => TestOutputs.WriteConnection(new BoxF());

    [TestMethod]
    public void TestConnectionExempleE2() => TestOutputs.WriteConnection(new BoxF2());
    [TestMethod]
    public void TestConnectionExempleE3() => TestOutputs.WriteConnection(new BoxF3());
    [TestMethod]
    public void TestConnectionWiringA() => TestOutputs.WriteConnection(new WiringA());
    [TestMethod]
    public void TestConnectionWiringB() => TestOutputs.WriteConnection(new WiringB());
    [TestMethod]
    public void TestConnectionWiringAAA() => TestOutputs.WriteConnection(new BoxAAA());
}