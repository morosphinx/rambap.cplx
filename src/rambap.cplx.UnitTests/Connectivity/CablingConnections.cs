using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplx.UnitTests.Connectivity;

class CablingConnections : Part
{
}


class Bench1 : Part, IPartConnectable
{
    Rack1 Rack1, Rack2;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ConnectWith(CableA, Rack1.EQ_PA.Port, Rack2.EQ_PA.Port);
        Do.ConnectWith(CableB, Rack1.EQ_PB.Port, Rack2.EQ_PB.Port);
    }
}
class Bench2 : Part, IPartConnectable
{
    Rack2 Rack1, Rack2;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ConnectWith(CableA, Rack1.InterfaceA, Rack2.InterfaceA);
        Do.ConnectWith(CableB, Rack1.InterfaceB, Rack2.InterfaceB);
    }
}
class Bench3 : Part, IPartConnectable
{
    Rack1 Rack1;
    Rack2 Rack2;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ConnectWith(CableA, Rack1.EQ_PA.Port, Rack2.InterfaceA);
        Do.ConnectWith(CableB, Rack1.EQ_PB.Port, Rack2.InterfaceB);
    }
}

class Rack1 : Part
{
    public Equipement1 EQ_PA;
    public Equipement1 EQ_PB;
}

class Rack2 : Part, IPartConnectable
{
    Equipement1 eqA;
    Equipement1 eqB;
    public ConnectablePort InterfaceA;
    public ConnectablePort InterfaceB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(eqA.Port, InterfaceA);
        Do.ExposeAs(eqB.Port, InterfaceB);
    }
}

class Equipement1 : Part
{
    public ConnectablePort Port;
}

class CableTypeA : Part
{
    public ConnectablePort L;
    public ConnectablePort R;
}

class CableTypeB : Part
{
    public ConnectablePort L;
    public ConnectablePort R;
}

[TestClass]
public class CablingConnectionsTests
{
    private void WriteConnection(Part part)
    {
        var instance = new Pinstance(part);

        var table = new TextTableFile(instance)
        {
            Table = ConnectivityTables.ConnectionTable(),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table.WriteToConsole();
    }

    [TestMethod] public void WriteBench1() => WriteConnection(new Bench1());
    [TestMethod] public void WriteBench2() => WriteConnection(new Bench2());
    [TestMethod] public void WriteBench3() => WriteConnection(new Bench3());
}