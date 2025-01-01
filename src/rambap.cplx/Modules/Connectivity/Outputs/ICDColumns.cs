using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    public static DelegateColumn<ComponentContent> TopMostConnectorName()
        => new DelegateColumn<ComponentContent>(
            "TopMostConnector",
            ColumnTypeHint.String,
            i => ((i as LeafProperty)?.Property as ICDTableContentProperty)!.Port.Name);
}
