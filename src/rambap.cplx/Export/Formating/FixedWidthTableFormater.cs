using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public class FixedWidthTableFormater : ITableFormater
{
    public string CellSeparator { get; set; } = "\t";
    public char CellPadding { get; set; } = ' ';
    public IEnumerable<string> Format(ITable table, Pinstance content)
    {
        IEnumerable<Line> cellTexts =
            [
                table.MakeHeaderLine(),
                .. table.MakeContentLines(content),
            ];
        var columnWidths = Support.CalculateColumnWidths(cellTexts);
        var columnIndexesToLeftPad = table.IColumns.Select(c => c.TypeHint == ColumnTypeHint.Numeric).ToList();
        var linesText = cellTexts.Select(l => Support.AggregateCells_FixedWidth(l, columnWidths, columnIndexesToLeftPad, CellSeparator, CellPadding));
        return linesText;
    }
}


