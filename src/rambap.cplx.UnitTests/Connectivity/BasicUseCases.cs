using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.UnitTests.Connectivity;

[TestClass]
public class BasicUseCases
{
    [TestMethod]
    public void PinAlone() => TestOutputs.WriteConnection<PinPart>();
    class PinPart : Pin
    {

    }

    [TestMethod]
    public void PinAloneExposed() => TestOutputs.WriteConnection<PinPart>();
    class PinAloneExposedPart : Part
    {
        public ConnectablePort J01 => P01;
        public WireablePort J02 => P01;

        PinPart P01;
    }

    [TestMethod]
    public void TwoPinWired() => TestOutputs.WriteConnection<TwoPinWiredPart>();
    class TwoPinWiredPart : Part, IPartConnectable
    {
        public PinPart A, B;
        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(A, B);
        }
    }

    [TestMethod]
    public void TwoPinMated() => TestOutputs.WriteConnection<TwoPinMatedPart>();
    class TwoPinMatedPart : Part, IPartConnectable
    {
        public PinPart A, B;
        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Mate(A, B);
        }
    }

    [TestMethod]
    public void ConnectorAlone() => TestOutputs.WriteConnection<ConnectorPart>();
    class ConnectorPart : Connector<PinPart>
    {
        public ConnectorPart() : base(9){}
    }

    [TestMethod]
    public void ConnectorExposed() => TestOutputs.WriteConnection<ConnectorExposedPart>();
    class ConnectorExposedPart : Part
    {
        public ConnectablePort J01 => P01;
        // public WireablePort DCD => P01.Pin(1);
        // public WireablePort Rx => P01.Pin(2);
        // public WireablePort Tx => P01.Pin(3);
        // public WireablePort DTR => P01.Pin(4);

        ConnectorPart P01;
    }

    [TestMethod]
    public void LoopbackConnector() => TestOutputs.WriteConnection<LoopbackConnectorPart>();
    class LoopbackConnectorPart : Part, IPartConnectable
    {
        public ConnectablePort J01 => C01.MateFace;

        ConnectorPart C01;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(C01.Pin(1), C01.Pin(4));
            Do.Wire(C01.Pin(2), C01.Pin(3));
        }
    }

    [TestMethod]
    public void SimpleCable() => TestOutputs.WriteConnection<SimpleCablePart>();
    class SimpleCablePart : Part, IPartConnectable
    {
        public ConnectablePort P01 => C01.MateFace;
        public ConnectablePort P02 => C02.MateFace;

        ConnectorPart C01;
        ConnectorPart C02;

        // Signal zeze => P01;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(C01.Pin(1), C02.Pin(4));
            Do.Wire(C01.Pin(2), C02.Pin(3));
        }
    }

    [TestMethod]
    public void SimpleBlackbox() => TestOutputs.WriteConnection<SimpleBlackboxPart>();
    class SimpleBlackboxPart : Part
    {
        public ConnectablePort J01 => C01.MateFace;
        public ConnectablePort J02;

        ConnectorPart C01;
    }

    [TestMethod]
    public void SimpleAssembly() => TestOutputs.WriteConnection<SimpleAssemblyPart>();
    class SimpleAssemblyPart : Part, IPartConnectable
    {
        SimpleBlackboxPart U01;
        SimpleBlackboxPart U02;
        SimpleCablePart W01;
        SimpleCablePart W02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            // Cabling
            Do.CableWith(W01, U01.J01, U02.J01);
            // Equivalent double Mating
            Do.Mate(U01.J02, W02.P01);
            Do.Mate(W02.P02, U02.J02);
        }
    }

    [TestMethod]
    public void TwistedCable() => TestOutputs.WriteConnection<TwistedCablePart>();
    class TwistedCablePart : Part, IPartConnectable
    {
        public ConnectablePort P01 => C01.MateFace;
        public ConnectablePort P02 => C02.MateFace;

        ConnectorPart C01;
        ConnectorPart C02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Twist([
                Do.Wire(C01.Pin(1), C02.Pin(4)),
                Do.Wire(C01.Pin(2), C02.Pin(3)),
            ]);
        }
    }
}
