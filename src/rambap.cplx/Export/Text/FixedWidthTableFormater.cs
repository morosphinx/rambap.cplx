using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Export.Text;

public class FixedWidthTableFormater : ITableFormater
{
    public string CellSeparator { get; set; } = "\t";
    public char CellPadding { get; set; } = ' ';
    public IEnumerable<string> Format(ITableProducer table, Component content)
    {
        IEnumerable<Line> cellTexts = table.MakeAllLines(content);
        var columnWidths = Support.CalculateColumnWidths(cellTexts.Select(t => t.Cells));
        var columnIndexesToLeftPad = table.IColumns.Select(c => c.TypeHint == ColumnTypeHint.Numeric).ToList();
        var linesText = cellTexts.Select(l => Support.AggregateCells_FixedWidth(l, columnWidths, columnIndexesToLeftPad, CellSeparator, CellPadding));
        return linesText;
    }
}


