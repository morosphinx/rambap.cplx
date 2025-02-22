using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

public class MarkdownTableFormater : ITableFormater
{
    public const string CellSeparator = "|";
    public char CellPadding { get; set; } = ' ';

    private Line MakeSeparatorLine(ITableProducer table) =>
        new()
        {
            Type = Line.LineType.Spacer,
            Cells = Enumerable.Repeat("-", table.IColumns.Count()).ToList(),
        };

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
        var lines = table.MakeAllLines(content);
        if (!lines.Any()) return [];

        var markdownTableHeaderSeparator = MakeSeparatorLine(table);

        IEnumerable<Line> MarkdownLineStructure(IEnumerable<Line> lines)
        {
            Line.LineType previousType = (Line.LineType) (-1);
            // Add a Header if missing => required for formating markdown table
            if (lines.First().Type != Line.LineType.Header)
            {
                yield return table.MakeHeaderLine();
                yield return markdownTableHeaderSeparator;
                previousType = Line.LineType.Header;
            }
            // Content lines may have markdown separator appended
            foreach (var line in lines)
            {
                switch (line.Type)
                {
                    case Line.LineType.Header:
                        yield return line;
                        yield return markdownTableHeaderSeparator;
                        break;
                    case Line.LineType.Total:
                        if (previousType == Line.LineType.Header)
                        {
                            // Total on top, before data
                            yield return line;
                            yield return markdownTableHeaderSeparator;
                        }
                        else
                        {
                            // Total on bottom, after data
                            yield return markdownTableHeaderSeparator;
                            yield return line;
                        }
                        break;
                    case Line.LineType.TableBreak:
                        yield return line;
                        yield return table.MakeHeaderLine();
                        yield return markdownTableHeaderSeparator;
                        break;
                    default:
                        yield return line;
                        break;
                }
                previousType = line.Type;
            }
        }

        var markdownLines = MarkdownLineStructure(lines).ToList();

        var columnWidths = Support.CalculateColumnWidths(lines.Select(l => l.Cells));
        // Now that we know column size, update the separator line
        CompleteSeparatorLine(table, markdownTableHeaderSeparator, columnWidths);

        var columnIndexesToLeftPad = table.IColumns.Select(c => c.TypeHint == ColumnTypeHint.Numeric).ToList();

        IEnumerable<string> MarkdownFormating(IEnumerable<Line> lines)
        {
            foreach (var line in lines)
            {
                yield return line.Type switch
                {
                    Line.LineType.TableBreak => "",
                    _ => CellSeparator + Support.AggregateCells_FixedWidth(line, columnWidths, columnIndexesToLeftPad, CellSeparator, CellPadding) + CellSeparator
                };
            }
        }

        var lineStrings = MarkdownFormating(markdownLines);

        return lineStrings;
    }
}
