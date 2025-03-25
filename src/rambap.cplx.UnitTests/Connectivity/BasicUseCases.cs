using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.Modules.Connectivity.Templates;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;

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
    class LoopbackConnectorPart : Part, IPartConnectable, IPartSignalDefining
    {
        public ConnectablePort J01 => C01.MateFace;

        ConnectorPart C01;

        Signal GND;
        [Rename("3v3")]
        Signal _3V3;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(C01.Pin(1), C01.Pin(4));
            Do.Wire(C01.Pin(2), C01.Pin(3));
        }

        public void Assembly_Signals(SignalBuilder Do)
        {
            Do.Assign(GND, C01.Pin(1));
            Do.Assign(_3V3, C01.Pin(2));
        }
    }

    [TestMethod]
    public void SimpleCable() => TestOutputs.WriteConnection<SimpleCablePart>();
    class SimpleCablePart : Part, IPartConnectable, IPartSignalDefining
    {
        public ConnectablePort P01 => C01.MateFace;
        public ConnectablePort P02 => C02.MateFace;

        ConnectorPart C01;
        ConnectorPart C02;

        Signal TX => (Signal)C01.Pin(1);
        Signal RX;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(C01.Pin(1), C02.Pin(4));
            Do.Wire(C01.Pin(2), C02.Pin(3));
        }

        public void Assembly_Signals(SignalBuilder Do)
        {
            // Do.Assign(TX, C01.Pin(1));
            Do.Assign(RX, C01.Pin(2));
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

    [TestMethod]
    public void PredefinedSignalConnector() => TestOutputs.WriteConnection<PredefinedSignalConnectorPart>();
    class PredefinedSignalConnectorPart : Connector<PinPart>, IPartSignalDefining
    {
        public PredefinedSignalConnectorPart() : base(9){}

        public Signal DCD, RxD, TxD, DTR, GND, DSR, RTS, CTS, RI;

        public void Assembly_Signals(SignalBuilder Do)
        {
            Do.Assign(DCD, Pin(1));
            Do.Assign(RxD, Pin(2));
            Do.Assign(TxD, Pin(3));
            Do.Assign(DTR, Pin(4));
            Do.Assign(GND, Pin(5));
            Do.Assign(DSR, Pin(6));
            Do.Assign(RTS, Pin(7));
            Do.Assign(CTS, Pin(8));
            Do.Assign(RI,  Pin(9));
        }
    }

    [TestMethod]
    public void PredefinedSignalConnector2() => TestOutputs.WriteConnection<PredefinedSignalConnector2Part>();
    class PredefinedSignalConnector2Part : Connector<PinPart>
    {
        public PredefinedSignalConnector2Part() : base(9) { }

        public Signal DCD => (Signal)Pin(1);
        public Signal RxD => (Signal)Pin(2);
        public Signal TxD => (Signal)Pin(3);
        public Signal DTR => (Signal)Pin(4);
        public Signal GND => (Signal)Pin(5);
        public Signal DSR => (Signal)Pin(6);
        public Signal RTS => (Signal)Pin(7);
        public Signal CTS => (Signal)Pin(8);
        public Signal RI  => (Signal)Pin(9);
    }

    [TestMethod]
    public void PredefinedPortBlackbox() => TestOutputs.WriteConnection<PredefinedPortBlackboxPart>();
    class PredefinedPortBlackboxPart : Part, IPartConnectable
    {
        PredefinedSignalConnectorPart C01;
        PredefinedSignalConnectorPart C02;
        PredefinedSignalConnectorPart C03;

        public ConnectablePort P01 => C01;
        public ConnectablePort P02 => C02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            foreach(var i in Enumerable.Range(1, C01.PinCount))
            {
                Do.Wire(C01.Pin(i), C02.Pin(i));
            }
        }
    }

    [TestMethod]
    public void PredefinedPortBlackbox2() => TestOutputs.WriteConnection<PredefinedPortBlackbox2Part>();
    class PredefinedPortBlackbox2Part : Part, IPartConnectable
    {
        PredefinedSignalConnector2Part C01;
        PredefinedSignalConnector2Part C02;

        public ConnectablePort P01 => C01;
        public ConnectablePort P02 => C02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {

            Do.Wire(C01.TxD, C02.RxD);
            Do.Wire(C01.RxD, C02.TxD);
            Do.Wire(C01.GND, C02.GND);
        }
    }
}
