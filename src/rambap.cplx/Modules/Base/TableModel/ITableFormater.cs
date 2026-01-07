using rambap.cplx.Instantiation;

namespace rambap.cplx.Modules.Base.TableModel;

/// <summary>
/// Define how to format table contents to a text file. <see cref="ITableProducer"/>
/// </summary>
public interface ITableFormater
{
    IEnumerable<string> Format(ITableProducer table, Component content);
}


