using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

using Line = List<string>;

public class MarkdownTableFormater : ITableFormater
{
    public const string CellSeparator = "|";
    public char CellPadding { get; set; } = ' ';


    public bool WriteTotalLine { get; set; } = false;
    public bool TotalLineOnTop { get; set; } = false;

    private Line MakeSeparatorLine(ITableProducer table) => Enumerable.Repeat("-", table.IColumns.Count()).ToList();

    private void CompleteSeparatorLine(ITableProducer table, Line separatorLine, List<int> columnCharWidth)
    {
        for (int i = 0; i < table.IColumns.Count(); i++)
        {
            if (columnCharWidth[i] == 0) separatorLine[i] = "";
            else if (columnCharWidth[i] == 1) separatorLine[i] = "-";
            else if (table.IColumns.ElementAt(i).TypeHint == ColumnTypeHint.Numeric)
            {
                // Rigth-Align numeric values
                separatorLine[i] = new string('-', Math.Max(1, columnCharWidth[i] - 1)) + ':';
            }
            else
            {
                // A full line of '-'
                separatorLine[i] = new string('-', Math.Max(2, columnCharWidth[i]));
            }
        }
    }

    public IEnumerable<string> Format(ITableProducer table, Pinstance content)
    {
        var separatorLineContent = MakeSeparatorLine(table);
        IEnumerable<Line> cellTexts;
        if (TotalLineOnTop)
        {
            cellTexts =
            [
                table.MakeHeaderLine(),
                .. WriteTotalLine
                    ? new List<Line>(){
                        separatorLineContent,
                        table.MakeTotalLine(content),}
                    :[],
                separatorLineContent,
                .. table.MakeContentLines(content),
            ];
        }
        else // Total line should be writen on bottom
        {
            cellTexts =
            [
                table.MakeHeaderLine(),
                separatorLineContent,
                .. table.MakeContentLines(content),
                .. WriteTotalLine
                    ? new List<Line>(){
                        separatorLineContent,
                        table.MakeTotalLine(content),}
                    :[],
            ];
        }
        var columnWidths = Support.CalculateColumnWidths(cellTexts);
        // Now that we know column size, update the separator line
        CompleteSeparatorLine(table, separatorLineContent, columnWidths);

        var columnIndexesToLeftPad = table.IColumns.Select(c => c.TypeHint == ColumnTypeHint.Numeric).ToList();

        var linesText = cellTexts.Select(l =>
            CellSeparator + Support.AggregateCells_FixedWidth(l, columnWidths, columnIndexesToLeftPad, CellSeparator, CellPadding) + CellSeparator);
        return linesText;
    }
}
