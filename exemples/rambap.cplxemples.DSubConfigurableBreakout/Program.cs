using rambap.cplx.Export;
using rambap.cplx.Export.Tables;
using rambap.cplx.Export.TextFiles;
using rambap.cplx.Modules.Connectivity.Outputs;
using System.Diagnostics;

namespace rambap.cplxexemples.DSubConfigurableBreakout;

internal class Program
{
    static void Main(string[] args)
    {
        // Define the Part
        var part = new CableBundle();

        // Calculate component tree and properties
        var part_instance = new Pinstance(part);

        var CorrectWiringTable = new TextTableFile(part_instance)
        {
            Formater = new MarkdownTableFormater(),
            Table = ConnectivityTables.WiringTable() with
            {
                Columns = []
            }
        };

        // Define what kind of files to generate
        var generator = Generators.ConfigureGenerator(
            [
                Generators.Content.SystemView,
                Generators.Content.Connectivity,
            ]
            , Generators.HierarchyMode.Flat);

        // Generate the files
        generator.Do(part_instance, "./Output");

        // Open the created folder
        Process.Start("explorer.exe", @".\Output");
    }
}

