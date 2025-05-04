using rambap.cplx.Core;

namespace rambap.cplx.Modules.Base.TableModel;

/// <summary>
/// Define how to create table contents. Used in tandem with an <see cref="ITableFormater"/>
/// </summary>
public interface ITableProducer
{
    /// <summary>
    /// Definition of the columns of the table
    /// </summary>
    public IEnumerable<IColumn> IColumns { get; }

    public IEnumerable<Line> MakeAllLines(Component rootComponent);

    public Line MakeHeaderLine();
    public IEnumerable<Line> MakeContentLines(Component rootComponent);
    public Line MakeTotalLine(Component rootComponent);
}