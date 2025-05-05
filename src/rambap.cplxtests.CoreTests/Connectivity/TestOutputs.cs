using rambap.cplx.Export.CoreTables;
using rambap.cplx.Export.Text;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplxtests.CoreTests.Connectivity;

internal static class TestOutputs
{
    internal static void WriteConnection<T>()
        where T : Part, new()
    {
        var part = new T();
        WriteConnection(part.Instantiate());
    }

    internal static void WriteConnection(Part part)
        => WriteConnection(part.Instantiate());
    internal static void WriteConnection(Component component)
    {
        Console.WriteLine("");
        Console.WriteLine($"{component.PN}");

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
        var connectiontable = new ConnectionTable();
        AddDebugInfoTo(connectiontable);
        var connectionFile = new TxtTableFile(component)
        {
            Table = connectiontable,
            Formater = new MarkdownTableFormater()
        };
        connectionFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("Wirings");
        var wiringtable = new WiringTable();
        AddDebugInfoTo(wiringtable);
        var wiringFile = new TxtTableFile(component)
        {
            Table = wiringtable,
            Formater = new MarkdownTableFormater()
        };
        wiringFile.WriteToConsole();

        Console.WriteLine("");
        Console.WriteLine("ICD");
        var ICDtable = new PortICD();
        AddDebugInfoTo(ICDtable);
        var ICDfile = new TxtTableFile(component)
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
        var c = p.Instantiate();
        var benchComponent = c.SubComponents.First();
        WriteConnection(benchComponent);
    }
}