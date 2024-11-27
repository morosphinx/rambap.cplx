using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;

public class CSVTableFile : AbstractTableFile
{
    public string CellSeparator { get; init; } = "\t";
    public CSVTableFile(Pinstance content) : base(content) { }
    public override void Do(string path)
    {
        IEnumerable<Line> cellTexts =
            [
                Table.MakeHeaderLine(),
                .. Table.MakeContentLines(Content),
            ];
        var linesText = cellTexts.Select(l => AggregateCells(l, CellSeparator));
        File.WriteAllLines(path, linesText);
    }
}


