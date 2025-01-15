using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.UnitTests.Connectivity;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.UnitTests.Connectivity;

class CablingConnections : Part
{
}


class Bench1 : Part, IPartConnectable
{
    Rack1 RackLL, RackRR;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(CableA, RackLL.EQ_PA.Port, RackRR.EQ_PA.Port);
        Do.CableWith(CableB, RackLL.EQ_PB.Port, RackRR.EQ_PB.Port);
    }
}
class Bench2 : Part, IPartConnectable
{
    Rack2 RackLL, RackRR;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(CableA, RackLL.InterfaceA, RackRR.InterfaceA);
        Do.CableWith(CableB, RackLL.InterfaceB, RackRR.InterfaceB);
    }
}
class Bench3 : Part, IPartConnectable
{
    Rack1 RackLL;
    Rack2 RackRR;
    CableTypeA CableA;
    CableTypeB CableB;
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.CableWith(CableA, RackLL.EQ_PA.Port, RackRR.InterfaceA);
        Do.CableWith(CableB, RackLL.EQ_PB.Port, RackRR.InterfaceB);
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
    private void WriteConnection(ConnectorIdentity displayIdentity, Part part)
        => WriteConnection(displayIdentity, new Pinstance(part));
    private void WriteConnection(ConnectorIdentity displayIdentity, Pinstance instance)
    {
        var table = new TextTableFile(instance)
        {
            Table = ConnectivityTables.ConnectionTable(displayIdentity),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table.WriteToConsole();
    }

    [TestMethod] public void WriteBench1_IM() => WriteConnection(ConnectorIdentity.Immediate, new Bench1());
    [TestMethod] public void WriteBench1_TOP() => WriteConnection(ConnectorIdentity.Topmost, new Bench1());
    [TestMethod] public void WriteBench2_IM() => WriteConnection(ConnectorIdentity.Immediate, new Bench2());
    [TestMethod] public void WriteBench2_TOP() => WriteConnection(ConnectorIdentity.Topmost, new Bench2());
    [TestMethod] public void WriteBench3_IM() => WriteConnection(ConnectorIdentity.Immediate, new Bench3());
    [TestMethod] public void WriteBench3_TOP() => WriteConnection(ConnectorIdentity.Topmost, new Bench3());


    class BenchWrapper<T> : Part
        where T : Part
    {
        public T Bench;
    }

    [TestMethod]
    public void WriteWrappedBench3_IM()
    {
        var p = new BenchWrapper<Bench3>();
        var i = new Pinstance(p);
        var benchInstance = i.Components.First().Instance;
        WriteConnection(ConnectorIdentity.Immediate, benchInstance);
    }

    [TestMethod] 
    public void WriteWrappedBench3_TOP()
    {
        var p = new BenchWrapper<Bench3>();
        var i = new Pinstance(p);
        var benchInstance = i.Components.First().Instance;
        WriteConnection(ConnectorIdentity.Topmost, benchInstance);
    }


    private void WriteICD(ConnectorIdentity displayIdentity, Part part)
    => WriteICD(displayIdentity, new Pinstance(part));
    private void WriteICD(ConnectorIdentity displayIdentity, Pinstance instance)
    {
        var table = new TextTableFile(instance)
        {
            Table = ConnectivityTables.InterfaceControlDocumentTable(),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table.WriteToConsole();
    }

    [TestMethod] public void WriteRack1_ICD() => WriteICD(ConnectorIdentity.Immediate, new Rack1());
    [TestMethod] public void WriteRack2_ICD() => WriteICD(ConnectorIdentity.Topmost, new Rack2());
}

class Bench4 : Bench3, IPartAdditionalDocuments
{
    public void Additional_Documentation(DocumentationBuilder Do)
    {
        Do.AddInstruction(i =>
        {
            var textFile = new TextTableFile(i)
            {
                Table = ConnectivityTables.ConnectionTable(ConnectorIdentity.Immediate),
                Formater = new Export.Tables.MarkdownTableFormater()
            };
            return ("TestFilename", textFile);
        });
    }
}