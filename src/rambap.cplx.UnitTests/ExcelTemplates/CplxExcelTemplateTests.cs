using rambap.cplx.Modules.Costing.Outputs;
using rambap.cplx.Export;
using rambap.cplx.Export.Spreadsheet;
using static rambap.cplx.Export.Generators;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Export.Tables;

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

    public static IGenerator CplxCostingGenerator(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [
                ($"Costing_{IGenerator.SimplefileNameFor(i)}.xlsx", CplxCostingTemplate(i))
                ],
            HierarchyMode.Flat, c => fileContentRecursion);
    }

    [TestMethod]
    public void TestCplxCostingGenerator()
    {
        var p = new BreakoutBox1();
        var i = new Pinstance(p);
        var generator = CplxCostingGenerator();
        generator.Do(i, "C:\\TestFolder\\Breakout9_Costing");
    }


    public static IInstruction CustomCostingTemplate(Pinstance i)
    {
        var CustomPartTable = CostTables.BillOfMaterial() with
        {
            Columns = [
                    IDColumns.PartNumber(),
                    CommonColumns.EmptyColumn(), // Not filled by CPLX
                    CostColumns.CostName(),
                    CostColumns.UnitCost(),
                    CommonColumns.ComponentTotalCount(),
                    CostColumns.TotalCost(),
                ]
        };
        var CustomTaskTable = TaskTables.BillOfTasks() with
        {
            Columns = [
                    IDColumns.PartNumber(),
                    TaskColumns.TaskName(),
                    TaskColumns.TaskCategory(),
                    TaskColumns.TaskDuration(),
                    TaskColumns.TaskRecurence(),
                    TaskColumns.TaskCount(),
                    TaskColumns.TaskTotalDuration(true)
                ]
        };

        bool IsDevTask(InstanceTasks.NamedTask task)
            => task.Category.ToLower().Contains("soft");
        ITableProducer CustomDevTaskTable = CustomTaskTable with
        {
            ContentTransform = l => l.Where(c => c switch
            {
                LeafProperty p when p.Property is InstanceTasks.NamedTask t => IsDevTask(t),
                _ => true,
            })
        };
        ITableProducer CustomDevNonTaskTable = CustomTaskTable with
        {
            ContentTransform = l => l.Where(c => c switch
            {
                LeafProperty p when p.Property is InstanceTasks.NamedTask t => !IsDevTask(t),
                _ => true,
            })
        };

        return new ExcelTableFile_FromTemplate(i)
        {
            TemplatePath = "ExcelTemplates\\CustomCostingTemplate.xlsx",
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
                        Table = CustomPartTable
                    },
                    new TableWriteInstruction(){
                        SheetName = "DeveloperTasks",
                        ColStart = 1,
                        RowStart = 2,
                        Table = CustomDevTaskTable
                    },
                    new TableWriteInstruction(){
                        SheetName = "OtherTasks",
                        ColStart = 1,
                        RowStart = 2,
                        Table = CustomDevNonTaskTable
                    }
                ]
        };
    }


    public static IGenerator CustomCostingGenerator(bool fileContentRecursion = false)
    {
        return Generators.ConfigureGenerator(
            i => [
                ($"CustomCosting_{IGenerator.SimplefileNameFor(i)}.xlsx", CustomCostingTemplate(i))
                ],
            HierarchyMode.Flat, c => fileContentRecursion);
    }

    [TestMethod]
    public void TestCustomCostingGenerator()
    {
        var p = new BreakoutBox1();
        var i = new Pinstance(p);
        var generator = CustomCostingGenerator();
        generator.Do(i, "C:\\TestFolder\\Breakout9_CustomCosting");
    }
}





