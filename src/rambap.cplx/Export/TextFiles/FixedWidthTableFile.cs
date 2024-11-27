﻿using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public class FixedWidthTableFile : AbstractTableFile
{
    public string CellSeparator { get; set; } = "\t";
    public char CellPadding { get; set; } = ' ';
    public FixedWidthTableFile(Pinstance content) : base(content) { }
    public override void Do(string path)
    {
        IEnumerable<Line> cellTexts =
            [
                Table.MakeHeaderLine(),
                .. Table.MakeContentLines(Content),
            ];
        var columnWidths = CalculateColumnWidths(cellTexts);
        var linesText = cellTexts.Select(l => AggregateCells_FixedWidth(l, columnWidths, CellSeparator, CellPadding));
        File.WriteAllLines(path, linesText);
    }
}


