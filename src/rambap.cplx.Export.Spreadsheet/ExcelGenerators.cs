using rambap.cplx.Concepts.Costing.Outputs;
using rambap.cplx.Core;
using rambap.cplx.Export.Spreadsheet;

namespace rambap.cplx.Export;

public class ExcelGenerators
{
    public static IEnumerable<(string, IInstruction)> CostingFiles(Pinstance i, string filenamePattern)
    {
        return [
                ($"BOMR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = CostTables.BillOfMaterial() }),
                ($"Costs_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = CostTables.CostBreakdown() }),
                ($"BOTR_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = TaskTables.BillOfTasks()}),
                ($"Tasks_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = TaskTables.RecurentTaskBreakdown() }),
                ];
    }

    public static IEnumerable<(string, IInstruction)> SystemViewTables(Pinstance i, string filenamePattern)
    {
        return [
                ($"Tree_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Concepts.Documentation.Outputs.SystemViewTables.ComponentTree() }),
                ($"Inventory_{filenamePattern}.xlsx", new ExcelTableFile_CreatedNew(i) { Table = Concepts.Documentation.Outputs.SystemViewTables.ComponentInventory() }),
                ];
    }
}

