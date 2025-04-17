using rambap.cplx.Export;
using System.Diagnostics;

namespace rambap.cplx.Template.Exe;

internal class Program
{
    static void Main(string[] args)
    {
        // Define the Part
        var part = new MyPart();

        // Calculate component tree and properties
        var part_instance = part.Instantiate();

        // Define what kind of files to generate
        var generator = Generators.ConfigureGenerator(Generators.Content.Costing, Generators.HierarchyMode.Flat);

        // Generate the files
        generator.Do(part_instance, "./Output");

        // Open the created folder
        Process.Start("explorer.exe", @".\Output");
    }
}

