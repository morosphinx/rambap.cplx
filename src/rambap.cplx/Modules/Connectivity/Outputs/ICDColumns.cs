using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    public static DelegateColumn<ComponentContent> TopMostConnectorName()
        => new DelegateColumn<ComponentContent>(
            "TopMostConnector",
            ColumnTypeHint.String,
            i => i switch
            {
                BranchComponent b => b.Component.CN,
                LeafProperty { Property : ICDTableContentProperty prop} p => prop.Port.Name,
                _ => throw new NotImplementedException(),
            });
}
