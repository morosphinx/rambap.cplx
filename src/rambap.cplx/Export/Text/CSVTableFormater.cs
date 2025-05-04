using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Export.Text;

public class CSVTableFormater : ITableFormater
{
    public string CellSeparator { get; init; } = "\t";
    public IEnumerable<string> Format(ITableProducer table, Component content)
    {
        IEnumerable<Line> cellTexts = table.MakeAllLines(content);
        var linesText = cellTexts.Select(l => Support.AggregateCells(l, CellSeparator));
        return linesText;
    }
}


