using DocumentFormat.OpenXml.Drawing;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Base.Output;
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

        void AddDebugInfoTo(TableProducer<IComponentContent> tableProducer)
        {
            tableProducer.Columns.InsertRange(0,
            [
                CommonColumns.LineNumber(),
                IDColumns.ComponentNumberPrettyTree()
            ]);
        }

        Console.WriteLine("");
        Console.WriteLine("Connectivity");
        var connectiontable = ConnectivityTables.ConnectionTable(displayIdentity);
        AddDebugInfoTo(connectiontable);
        var connectionFile = new TextTableFile(instance)
        {
            Table = connectiontable,
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        connectionFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("Wirings");
        var wiringtable = ConnectivityTables.WiringTable();
        AddDebugInfoTo(wiringtable);
        var wiringFile = new TextTableFile(instance)
        {
            Table = wiringtable,
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        wiringFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("ICD");
        var ICDtable = ConnectivityTables.InterfaceControlDocumentTable();
        AddDebugInfoTo(ICDtable);
        var ICDfile = new TextTableFile(instance)
        {
            Table = ICDtable,
            Formater = new Export.Tables.MarkdownTableFormater()
        };
        ICDfile.WriteToConsole();
    }
}