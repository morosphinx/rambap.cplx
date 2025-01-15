using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using rambap.cplx.Modules.Connectivity.Templates;

namespace rambap.cplx.UnitTests.Connectivity;

interface IBlackboxes
{
    ConnectablePort PortA { get; set; }
}

class BlackBox_Pin : Pin { }
class BlackBox_Connector : Connector<BlackBox_Pin>
{
    const int p = 15;
    public BlackBox_Connector() : base(p)
    {
        this.PN = $"BlackBoxConnector_{p}";
    }
}

class BlackBox_Type1 : Part, IBlackboxes
{
    public ConnectablePort PortA { get; set; }
}
class BlackBox_Type2 : Part, IBlackboxes, IPartConnectable 
{
    public ConnectablePort PortA { get; set; }
    public void Assembly_Connections(ConnectionBuilder Do){}
}

#if false // The following syntax is currently not valid :
class BlackBox_Type3 : Part, IBlackboxes
{
    BlackBox_Connector Connector;
    public ConnectablePort PortA => Connector.MateFace;
}
class BlackBox_Type4 : Part, IBlackboxes
{
    public BlackBox_Connector Connector;
    public ConnectablePort PortA => Connector.MateFace;
}

class BlackBox_Type5 : Part, IBlackboxes, IPartConnectable
{
    BlackBox_Connector Connector;
    public ConnectablePort PortA => Connector.MateFace;

    public void Assembly_Connections(ConnectionBuilder Do){}
}

class BlackBox_Type6 : Part, IBlackboxes, IPartConnectable
{
    public BlackBox_Connector Connector;
    public ConnectablePort PortA => Connector.MateFace;
    public void Assembly_Connections(ConnectionBuilder Do) { }
}
#endif
class BlackBox_Type7 : Part, IBlackboxes, IPartConnectable
{
    BlackBox_Connector Connector;
    public ConnectablePort PortA { get; set; }
    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.ExposeAs(Connector.MateFace, PortA);
    }
}

class BlackBox_Type8 : Part
{
    public ConnectablePort AbstractPort;

    public BlackBox_Type7 SubBox;

    Part AbstractPrivatePart;
    public Part AbstractPublicPart;
    public HierPart part;
}
class HierPart : Part
{
    public ConnectedPart C01;
    ConnectedPart C02_private;
}
class ConnectedPart : Part
{
    public ConnectablePort APort;
}


[TestClass]
public class TestBlackBoxesICDs
{
    private void TestBlackBoxICD(Part b)
    {
        var i = new Pinstance(b);
        var file = new TextTableFile(i)
        {
            Formater = new FixedWidthTableFormater(),
            Table = ConnectivityTables.InterfaceControlDocumentTable(),
        };
        file.WriteToConsole();
    }

    [TestMethod] public void BB1() => TestBlackBoxICD(new BlackBox_Type1());
    [TestMethod] public void BB2() => TestBlackBoxICD(new BlackBox_Type2());
#if false
    [TestMethod] public void BB3() => TestBlackBoxICD(new BlackBox_Type3());
    [TestMethod] public void BB4() => TestBlackBoxICD(new BlackBox_Type4());
    [TestMethod] public void BB5() => TestBlackBoxICD(new BlackBox_Type5());
    [TestMethod] public void BB6() => TestBlackBoxICD(new BlackBox_Type6());
#endif
    [TestMethod] public void BB7() => TestBlackBoxICD(new BlackBox_Type7());
    [TestMethod] public void BB8() => TestBlackBoxICD(new BlackBox_Type8());
}