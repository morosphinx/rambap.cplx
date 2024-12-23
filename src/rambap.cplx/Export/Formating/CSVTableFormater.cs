using rambap.cplx.Core;

namespace rambap.cplx.Export.TextFiles;

using Line = List<string>;
public interface ITableFormater
{
    IEnumerable<string> Format(ITable table, Pinstance content);
}

public class CSVTableFormater : ITableFormater
{
    public string CellSeparator { get; init; } = "\t";
    public IEnumerable<string> Format(ITable table, Pinstance content)
    {
        IEnumerable<Line> cellTexts =
            [
                table.MakeHeaderLine(),
                .. table.MakeContentLines(content),
            ];
        var linesText = cellTexts.Select(l => Support.AggregateCells(l, CellSeparator));
        return linesText;
    }
}


