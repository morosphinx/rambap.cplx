using rambap.cplx.Export.Tables;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.Outputs;

namespace rambap.cplxtests.CoreTests.Connectivity;

internal static class TestOutputs
{
    internal static void WriteConnection<T>()
        where T : Part, new()
    {
        var part = new T();
        var i = new Pinstance(part);
        WriteConnection(i);
    }

    internal static void WriteConnection(Part part)
        => WriteConnection(new Pinstance(part));
    internal static void WriteConnection(Pinstance instance)
    {
        Console.WriteLine("");
        Console.WriteLine($"{instance.PN}");

        void AddDebugInfoTo(TableProducer<ICplxContent> tableProducer)
        {
            tableProducer.Columns.InsertRange(0,
            [
                CommonColumns.LineNumber(),
                IDColumns.ComponentNumberPrettyTree()
            ]);
        }

        Console.WriteLine("");
        Console.WriteLine("Connectivity");
        var connectiontable = ConnectivityTables.ConnectionTable();
        AddDebugInfoTo(connectiontable);
        var connectionFile = new TxtTableFile(instance)
        {
            Table = connectiontable,
            Formater = new MarkdownTableFormater()
        };
        connectionFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("Wirings");
        var wiringtable = ConnectivityTables.WiringTable();
        AddDebugInfoTo(wiringtable);
        var wiringFile = new TxtTableFile(instance)
        {
            Table = wiringtable,
            Formater = new MarkdownTableFormater()
        };
        wiringFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("ICD");
        var ICDtable = ConnectivityTables.InterfaceControlDocumentTable();
        AddDebugInfoTo(ICDtable);
        var ICDfile = new TxtTableFile(instance)
        {
            Table = ICDtable,
            Formater = new MarkdownTableFormater()
        };
        ICDfile.WriteToConsole();
    }


    class HierarchyAbstractParentPart<T> : Part
        where T : Part
    {
        public T Bench;
    }

    internal static void WriteConnectionInCaseOfParent<T>()
        where T : Part, new()
    {
        var p = new HierarchyAbstractParentPart<T>();
        var i = new Pinstance(p);
        var benchInstance = i.Components.First().Instance;
        WriteConnection(benchInstance);
    }
}