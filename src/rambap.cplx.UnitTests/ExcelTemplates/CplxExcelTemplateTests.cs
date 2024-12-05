using rambap.cplx.Export;
using rambap.cplx.Export.Spreadsheet;
using rambap.cplx.Export.Tables;
using static rambap.cplx.Export.Generators;

namespace rambap.cplx.UnitTests.ExcelTemplates;

[TestClass]
public class CplxExcelTemplateTests
{
    public static IInstruction CplxCostingTemplate(Pinstance i)
    {
        return new ExcelTableFile_FromTemplate(i)
        {
            TemplatePath = "ExcelTemplates\\CplxCostingTemplate.xlsx",
            Tables = [
                    new TableWriteInstruction(){
                        SheetName = "Parts",
                        ColStart = 1,
                        RowStart = 2,
                        Table = Costing.BillOfMaterial()
                    },
                    new TableWriteInstruction(){
                        SheetName = "Tasks",
                        ColStart = 1,
                        RowStart = 2,
                        Table = Costing.BillOfTasks()
                    }
                ]
        };
    }

    public static IGenerator CostingGenerator(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [($"Costing_{IGenerator.SimplefileNameFor(i)}.xlsx", CplxCostingTemplate(i))],
            HierarchyMode.Flat, c => fileContentRecursion);
    }

    [TestMethod]
    public void TestCostingGenerator()
    {
        var p = new BreakoutBox1();
        var i = new Pinstance(p);
        var generator = CostingGenerator();
        generator.Do(i, "C:\\TestFolder\\Breakout9_Costing");
    }
}

