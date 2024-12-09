using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public class MarkdownTableFile : AbstractTableFile
{
    public const string CellSeparator = "|";
    public char CellPadding { get; set; } = ' ';
    public bool WriteTotalLine { get; set; } = false;

    public MarkdownTableFile(Pinstance content) : base(content) { }

    private Line MakeSeparatorLine => Enumerable.Repeat("-", Table.IColumns.Count()).ToList();

    private void CompleteSeparatorLine(Line separatorLine, List<int> columnCharWidth)
    {
        for (int i = 0; i < Table.IColumns.Count(); i++)
        {
            if (columnCharWidth[i] == 0) separatorLine[i] = "";
            else if (columnCharWidth[i] == 1) separatorLine[i] = "-";
            else if (Table.IColumns.ElementAt(i).TypeHint == ColumnTypeHint.Numeric)
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

    public override void Do(string path)
    {
        var separatorLineContent = MakeSeparatorLine;
        IEnumerable<Line> cellTexts =
            [
                Table.MakeHeaderLine(),
                separatorLineContent,
                .. Table.MakeContentLines(Content),
                .. WriteTotalLine
                    ? new List<Line>(){
                        separatorLineContent,
                        Table.MakeTotalLine(Content),}
                    :[]
            ];
        var columnWidths = CalculateColumnWidths(cellTexts);
        // Now that we know column size, update the separator line
        CompleteSeparatorLine(separatorLineContent, columnWidths);

        var columnIndexesToLeftPad = Table.IColumns.Select(c => c.TypeHint == ColumnTypeHint.Numeric).ToList();

        var linesText = cellTexts.Select(l =>
            CellSeparator + AggregateCells_FixedWidth(l, columnWidths, columnIndexesToLeftPad, CellSeparator, CellPadding) + CellSeparator);
        File.WriteAllLines(path, linesText);
    }
}
