using rambap.cplx.Core;
using rambap.cplx.Export.Spreadsheet;
using rambap.cplx.Export.Text;
using rambap.cplx.Export.CoreTables;

namespace rambap.cplx.Export;

public class ExcelGenerators
{
    public static IEnumerable<(string, IInstruction)> CostingFiles(Component c, string filenamePattern)
    {
        return [
                ($"BOMR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                {
                    Table = new BillOfMaterial()
                }),
                ($"RecurentCosts_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                {
                    Table = new CostBreakdown()
                }),
                ($"BOTR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                { 
                    Table = TaskTables.BillOfTasks()
                }),
                ($"Tasks_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                {
                    Table = TaskTables.TaskBreakdown()
                }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> SystemViewTables(Component c, string filenamePattern)
    {
        return [
                ($"Tree_Detailled_{filenamePattern}.csv", new ExcelTableFile_CreatedNew(c)
                {
                    Table = CoreTables.SystemViewTables.ComponentTree_Detailled()
                }),
                ($"Tree_Stacked_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                {
                    Table = CoreTables.SystemViewTables.ComponentTree_Stacked()
                }),
                ($"Inventory_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(c)
                {
                    Table = CoreTables.SystemViewTables.ComponentInventory()
                }),
                ];
    }
}

