using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    public static DelegateColumn<ComponentContent> TopMostPortName()
        => new DelegateColumn<ComponentContent>(
            "TopMostPort",
            ColumnTypeHint.String,
            i => i switch
            {
                BranchComponent b => b.Component.CN,
                LeafProperty { Property : ICDTableContentProperty prop} p => prop.Port.TopMostUser().Name,
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ComponentContent> MostRelevantPortName()
    => new DelegateColumn<ComponentContent>(
        "PortEXP",
        ColumnTypeHint.String,
        i => i switch
        {
            BranchComponent b => b.Component.CN,
            LeafProperty { Property: ICDTableContentProperty prop } p => prop.Port.TopMostRelevant().Name,
            _ => throw new NotImplementedException(),
        });

    public static DelegateColumn<ComponentContent> MostRelevantPortName_Regard()
    => new DelegateColumn<ComponentContent>(
        "ColEXP",
        ColumnTypeHint.String,
        i => i switch
        {
            BranchComponent b => b.Component.CN,
            LeafProperty { Property: ICDTableContentProperty prop } p => prop.Port.GetExpositionColumnName(),
            _ => throw new NotImplementedException(),
        });

    public static DelegateColumn<ComponentContent> SelfPortName()
        => new DelegateColumn<ComponentContent>(
            "PortSelf",
            ColumnTypeHint.String,
            i => i switch
            {
                BranchComponent b => b.Component.CN,
                LeafProperty { Property: ICDTableContentProperty prop } p => prop.Port.Name,
                _ => throw new NotImplementedException(),
            });
}
