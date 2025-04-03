using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

public class CSVTableFormater : ITableFormater
{
    public string CellSeparator { get; init; } = "\t";
    public IEnumerable<string> Format(ITableProducer table, Pinstance content)
    {
        IEnumerable<Line> cellTexts = table.MakeAllLines(content);
        var linesText = cellTexts.Select(l => Support.AggregateCells(l, CellSeparator));
        return linesText;
    }
}


