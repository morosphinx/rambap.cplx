using rambap.cplx.Modules.Costing.Outputs;
using rambap.cplx.Export;
using rambap.cplx.Export.Spreadsheet;
using static rambap.cplx.Export.Generators;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.UnitTests.ExcelTemplates;

[TestClass]
public class CplxExcelTemplateTests
{
    public static IInstruction CplxCostingTemplate(Pinstance i)
    {
        var a = CostTables.BillOfMaterial();
        var b = a with { Columns = [] };
        return new ExcelTableFile_FromTemplate(i)
        {
            TemplatePath = "ExcelTemplates\\CplxCostingTemplate.xlsx",
            InstanceContents = [
                    new InstanceContentInstruction(){
                        SheetName = "Overview",
                        ColStart = 2,
                        RowStart = 2,
                        Lines =[
                                i => i.PN,
                                i => i.Revision,
                                i => i.Version,
                                i => DateTime.Now.ToString()
                            ]
                    }
                ],
            Tables = [
                    new TableWriteInstruction(){
                        SheetName = "Parts",
                        ColStart = 1,
                        RowStart = 2,
                        Table = CostTables.BillOfMaterial()
                    },
                    new TableWriteInstruction(){
                        SheetName = "Tasks",
                        ColStart = 1,
                        RowStart = 2,
                        Table = TaskTables.BillOfTasks()
                    }
                ]
        };
    }

    public static IGenerator CostingGenerator(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [
                ($"Costing_{IGenerator.SimplefileNameFor(i)}.xlsx", CplxCostingTemplate(i))
                ],
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

