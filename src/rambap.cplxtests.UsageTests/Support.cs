using rambap.cplx.Export;
using static rambap.cplx.Export.Generators;

namespace rambap.cplxtests.UsageTests;

internal class Support
{
    public static IGenerator GetDemoGenerator_AllCoreTables(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            [Content.Connectivity, Content.Costing, Content.SystemView, Content.DocumentationAdditionalFiles]
            , HierarchyMode.Flat, c => fileContentRecursion);
    }

    public static IGenerator GetDemoGenerator_AllCoreTables_Excel(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [.. ExcelGenerators.CostingFiles(i, IGenerator.SimplefileNameFor(i)),
                  .. ExcelGenerators.SystemViewTables(i, IGenerator.SimplefileNameFor(i)) ]
            , HierarchyMode.Flat, c => fileContentRecursion);
    }

    public static IGenerator GetDemoGenerator_AllProdocs(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [
                ("MdSystemView.md", new cplx.Export.Prodocs.MdSystemView(i)),
                ("MdWiringPlan.md", new cplx.Export.Prodocs.MdWiringPlan(i)),
                ]
            , HierarchyMode.Flat, c => fileContentRecursion);
    }
}
