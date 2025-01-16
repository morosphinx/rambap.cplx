using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using static rambap.cplx.UnitTests.Connectivity.ConnectionAction;

namespace rambap.cplx.UnitTests.Connectivity;

internal class Part_InternalCable(bool internalConnected) : Part, IPartConnectable
{
    public ConnectablePort LeftCableConnector;
    public ConnectablePort RigthCableConnector;

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        if(internalConnected)
            Do.Mate(LeftCableConnector, RigthCableConnector);
    }
}

enum ConnectionAction
{
    Nothing,
    Mate,
    Expose,
}

internal class Part_ContainerBox(ConnectionAction actionOnL, ConnectionAction actionOnR, bool internalConnected) : Part, IPartConnectable
{
    public ConnectablePort LeftBoxConnector;
    public ConnectablePort RigthBoxConnector;

    Part_InternalCable Cable = new Part_InternalCable(internalConnected);

    private static void DoConnectionAction(ConnectionBuilder Do, ConnectionAction action, ConnectablePort A, ConnectablePort B)
    {
        switch (action)
        {
            case (ConnectionAction.Nothing):
                break;
            case (ConnectionAction.Mate):
                Do.Mate(A, B); // Connect the cable connector to the box connector in a nondescript way
                break;
            case (ConnectionAction.Expose):
                Do.ExposeAs(B, A); // Expose the cable connector on the box exterior
                break;
            default:
                throw new NotImplementedException();
        };
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        DoConnectionAction(Do, actionOnL, LeftBoxConnector, Cable.LeftCableConnector);
        DoConnectionAction(Do, actionOnR, RigthBoxConnector, Cable.RigthCableConnector);
    }
}

[TestClass]
public class TestSimpleCableContainer
{
    private void TestConnectionCount(
        ConnectionAction actionOnL, ConnectionAction actionOnR, bool internalConnected,
        int expectedBoxConnectionCount, bool expectedEndToEndLink)
    {
        var part = new Part_ContainerBox(actionOnL, actionOnR, internalConnected);
        var instance = new Pinstance(part);
        // Write the output table for reference
        var table = new TextTableFile(instance)
        {
            Table = ConnectivityTables.ConnectionTable(ConnectivityColumns.ConnectorIdentity.Immediate),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table.WriteToConsole();
        // Test Connectivity property value
        var connectivity = instance.Connectivity();
        //Assert.AreEqual(expectedBoxConnectionCount, connectivity!.Connections.Count);
        var connections = ConnectivityTableIterator.GetAllConnections(instance, true, ConnectivityTableIterator.ConnectionKind.Assembly);
        Assert.AreEqual(expectedBoxConnectionCount, connections.Count());
        
        // TODO : Assert End To end Link
        // if internal connected, assert B.LeftBoxConnector and B.RigthBoxConnector are connected
    }

    //// ---- TESTS WITH CABLE MAKING A CONNECTION
    // Does it make sens to sometime connect/mate and expose on the same connector ?
    // If we connect on a connector, it does not make sense to show it as a public part
    // Differentiate those use cases in the connector definition ?

    // No connection in box, both side
    [TestMethod] public void TestNNC() => TestConnectionCount(Nothing, Nothing, true, 1, false);

    // One side is connected internaly => No endToEnd connection, one connection to document
    // Expected Connections (NCC) :
    // LeftCableConnector   --------(Cable)--------  RightCableConnector
    // RightCableConnector  -----------------------  RightBoxConnector
    [TestMethod] public void TestNMC() => TestConnectionCount(Nothing, Mate, true, 2, false); 
    [TestMethod] public void TestMNC() => TestConnectionCount(Mate, Nothing, true, 2, false);

    // One side is exposed => No endToEnd connection, should document the intenrnal cable
    // Expected Connections (NEC) :
    // LeftCableConnector   --------(Cable)--------  RightBoxConnector(RightCableConnector)
    [TestMethod] public void TestNEC() => TestConnectionCount(Nothing, Expose, true, 1, false);
    [TestMethod] public void TestENC() => TestConnectionCount(Expose, Nothing, true, 1, false);


    // One side is exposed and one connected internaly => Has endToEnd connection, 1 connection to document
    // Expected Connections (CEC) :
    // LeftBoxConnector     -----------------------  LeftCableConnector
    // LeftCableConnector   --------(Cable)--------  RightBoxConnector(RightCableConnector)
    [TestMethod] public void TestMEC() => TestConnectionCount(Mate, Expose, true, 2, true);
    [TestMethod] public void TestEMC() => TestConnectionCount(Expose, Mate, true, 2, true);

    // Both side connected internaly => Has endToEnd connection, ONE connection to document
    // Expected Connections (CCC) :
    // LeftBoxConnector     -----------------------  LeftCableConnector
    // LeftCableConnector   --------(Cable)--------  RigthCableConnector
    // RigthCableConnector  -----------------------  RigthBoxConnector
    [TestMethod] public void TestMMC() => TestConnectionCount(Mate, Mate, true, 3, true);


    // Both side exposed => Has endToEnd connection, 1 connection to document
    // Expected Connections (EEC) :
    // LeftBoxConnector(LeftCableConnector)   --------(Cable)--------  RightBoxConnector(RightCableConnector)
    [TestMethod] public void TestEEC() => TestConnectionCount(Expose, Expose, true, 1, true);





    //// ---- TESTS WITH CABLE NOT MAKING A CONNECTION
    // Same number = above - 1, except never has endToEnd connection
    // TODO : Confirm : While there is no internal signal connection in the cable, it should not change its internal representation in the
    // Parent connection diagram.

    [TestMethod] public void TestNNN() => TestConnectionCount(Nothing, Nothing, false, 0, false);

    [TestMethod] public void TestNMN() => TestConnectionCount(Nothing, Mate, false, 1, false);
    [TestMethod] public void TestMNN() => TestConnectionCount(Mate, Nothing, false, 1, false);

    [TestMethod] public void TestNEN() => TestConnectionCount(Nothing, Expose, false, 0, false);
    [TestMethod] public void TestENN() => TestConnectionCount(Expose, Nothing, false, 0, false);

    [TestMethod] public void TestMEN() => TestConnectionCount(Mate, Expose, false, 1, false);
    [TestMethod] public void TestEMN() => TestConnectionCount(Expose, Mate, false, 1, false);

    [TestMethod] public void TestMMN() => TestConnectionCount(Mate, Mate, false, 2, false);

    [TestMethod] public void TestEEN() => TestConnectionCount(Expose, Expose, false, 0, false);
}