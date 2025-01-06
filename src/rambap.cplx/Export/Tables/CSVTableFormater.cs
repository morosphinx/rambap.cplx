using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

using Line = List<string>;

public class CSVTableFormater : ITableFormater
{
    public string CellSeparator { get; init; } = "\t";
    public IEnumerable<string> Format(ITableProducer table, Pinstance content)
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


