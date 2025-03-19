using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.UnitTests.Connectivity;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectionColumns;

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

class Rack2 : Part
{
    Equipement1 eqA;
    Equipement1 eqB;

    public ConnectablePort InterfaceA => eqA.Port;
    public ConnectablePort InterfaceB => eqB.Port;
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
    [TestMethod] public void WriteBench1() => TestOutputs.WriteConnection(new Bench1());
    [TestMethod] public void WriteBench2() => TestOutputs.WriteConnection(new Bench2());
    [TestMethod] public void WriteBench3() => TestOutputs.WriteConnection(new Bench3());

    [TestMethod] public void WriteRack1() => TestOutputs.WriteConnection(new Rack1());
    [TestMethod] public void WriteRack2() => TestOutputs.WriteConnection(new Rack2());

    [TestMethod]
    public void WriteWrappedBench3() => TestOutputs.WriteConnectionInCaseOfParent<Bench3>();
}

class Bench4 : Bench3, IPartAdditionalDocuments
{
    public void Additional_Documentation(DocumentationBuilder Do)
    {
        Do.AddInstruction(i =>
        {
            var textFile = new TextTableFile(i)
            {
                Table = ConnectivityTables.ConnectionTable(),
                Formater = new Export.Tables.MarkdownTableFormater()
            };
            return ("TestFilename", textFile);
        });
    }
}