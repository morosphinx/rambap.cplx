using rambap.cplx.Modules.Connectivity.Templates;

using static System.Drawing.Color;
using static rambap.cplxtests.CoreTests.Connectivity.TestOutputs;

namespace rambap.cplxtests.CoreTests.Connectivity.Wiring;

[TestClass]
public class PreWiredConnectors
{
    class ConnectorPin : Pin ;
    class ConnectorFace : Connector<ConnectorPin>
    {
        public ConnectorFace() : base(9){}
    }
    class PreWire : WireSpool;
    class PreWiredConnector : Part, IPartConnectable
    {
        ConnectorFace U01;

        public ConnectablePort MateFace => U01.MateFace;
        public WireEnd W1;
        public WireEnd W2;
        public WireEnd W3;
        public WireEnd W4;
        public WireEnd W5;
        public WireEnd W6;
        public WireEnd W7;
        public WireEnd W8;
        public WireEnd W9;


        WireSpool WireSpool = new PreWire()
        {
            Diameter = WireSpool.WireDiameter.AWG24,
            Color = Gray,
            UnitPackingLength = 10
        };

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            IEnumerable<WireEnd> wireEnds = [W1, W2, W3, W4, W5, W6, W7, W8, W9];
            int pin = 1;
            foreach (var wireEnd in wireEnds)
            {
                Do.WirePre(U01.Pin(pin++), wireEnd);
            }
        }
    }


    class Connector_to_Connector_Assembly : Part, IPartConnectable
    {
        ConnectorFace U01;
        ConnectorFace U02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(U01.Pin(2), U02.Pin(3));
            Do.Wire(U01.Pin(3), U02.Pin(2));
            Do.Wire(U01.Pin(5), U02.Pin(5));
        }
    }
    [TestMethod]
    public void Test_Connector_to_Connector_Assembly()
    {
        var part = new Connector_to_Connector_Assembly();
        WriteConnection(part);
    }

    class PreWired_to_Connector_Assembly : Part, IPartConnectable
    {
        PreWiredConnector U01;
        ConnectorFace U02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(U01.W2, U02.Pin(3));
            Do.Wire(U01.W3, U02.Pin(2));
            Do.Wire(U01.W5, U02.Pin(5));
        }
    }
    [TestMethod]
    public void Test_PreWired_to_Connector_Assembly()
    {
        var part = new PreWired_to_Connector_Assembly();
        WriteConnection(part);
    }

    class PreWired_to_PeWired_Assembly : Part, IPartConnectable
    {
        PreWiredConnector U01;
        PreWiredConnector U02;

        public void Assembly_Connections(ConnectionBuilder Do)
        {
            Do.Wire(U01.W2, U02.W3);
            Do.Wire(U01.W3, U02.W2);
            Do.Wire(U01.W5, U02.W5);
        }
    }
    [TestMethod]
    public void Test_PreWired_to_PeWired_Assembly()
    {
        var part = new PreWired_to_PeWired_Assembly();
        WriteConnection(part);
    }
}
