using rambap.cplx.Core;

namespace rambap.cplx.Export.Tables;

public interface ITableFormater
{
    IEnumerable<string> Format(ITableProducer table, Pinstance content);
}


