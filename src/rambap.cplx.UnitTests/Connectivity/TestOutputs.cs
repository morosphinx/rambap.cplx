using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;

namespace rambap.cplx.UnitTests.Connectivity;

internal static class TestOutputs
{
    internal static void WriteConnection<T>()
        where T : Part, new()
    {
        var pin = new T();
        var i = new Pinstance(pin);
        TestOutputs.WriteConnection(ConnectorIdentity.Topmost, i);
        TestOutputs.WriteConnection(ConnectorIdentity.Immediate, i);
    }

    internal static void WriteConnection(ConnectorIdentity displayIdentity, Part part)
        => WriteConnection(displayIdentity, new Pinstance(part));
    internal static void WriteConnection(ConnectorIdentity displayIdentity, Pinstance instance)
    {
        Console.WriteLine("");
        Console.WriteLine($"{instance.PN} / {displayIdentity}");

        Console.WriteLine("");
        Console.WriteLine("Connectivity");
        var table1 = new TextTableFile(instance)
        {
            Table = ConnectivityTables.ConnectionTable(displayIdentity),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table1.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("Wirings");
        var table2 = new TextTableFile(instance)
        {
            Table = ConnectivityTables.WiringTable(),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table2.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("ICD");
        var table3 = new TextTableFile(instance)
        {
            Table = ConnectivityTables.InterfaceControlDocumentTable(),
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        table3.WriteToConsole();
    }
}