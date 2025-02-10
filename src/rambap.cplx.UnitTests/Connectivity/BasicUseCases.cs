﻿using rambap.cplx.Modules.Connectivity.Templates;
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
        public ConnectablePort J01 ; //=> C01.MateFace;

        ConnectorPart C01;

        public void Assembly_Ports(PortBuilder Do)
        {
            Do.ExposeAs(C01, J01);
        }

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(C01.Pin(1), C01.Pin(4));
            Do.Wire(C01.Pin(2), C01.Pin(3));
        }
    }
}
