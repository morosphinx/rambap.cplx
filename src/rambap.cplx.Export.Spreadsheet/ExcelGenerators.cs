using rambap.cplx.Core;
using rambap.cplx.Export.Spreadsheet;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export;

public class ExcelGenerators
{
    public static IEnumerable<(string, IInstruction)> CostingFiles(Pinstance i, string filenamePattern)
    {
        return [
                ($"BOMR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Costing.BillOfMaterial() }),
                ($"Costs_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Costing.CostBreakdown() }),
                ($"BOTR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Costing.BillOfTasks()}),
                ($"Tasks_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Costing.RecurentTaskBreakdown() }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> SystemViewTables(Pinstance i, string filenamePattern)
    {
        return [
                ($"Tree_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = SystemView.ComponentTree() }),
                ($"Inventory_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = SystemView.ComponentInventory() }),
                ];
    }
}

