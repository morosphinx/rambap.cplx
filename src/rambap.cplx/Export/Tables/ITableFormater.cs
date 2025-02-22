using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

/// <summary>
/// Define how to format table contents to a text file. <see cref="ITableProducer"/>
/// </summary>
public interface ITableFormater
{
    IEnumerable<string> Format(ITableProducer table, Pinstance content);
}


