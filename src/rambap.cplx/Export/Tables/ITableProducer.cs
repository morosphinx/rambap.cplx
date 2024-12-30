using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

using Line = List<string>;

public interface ITableProducer
{
    public IEnumerable<IColumn> IColumns { get; }

    Line MakeHeaderLine();
    IEnumerable<Line> MakeContentLines(Pinstance rootComponent);
    Line MakeTotalLine(Pinstance rootComponent);
}

