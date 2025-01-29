using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    // Port names are displayed as exact strings, no formating

    public static DelegateColumn<IComponentContent> TopMostPortPart()
    => new DelegateColumn<IComponentContent>(
        "Part",
        ColumnTypeHint.StringExact,
        i => i switch
        {
            BranchComponent b => b.Component.CN,
            IPropertyContent { Property: ICDTableContentProperty prop } p => prop.Port.GetTopMostUser().Owner?.ImplementingInstance.CN ?? "",
            LeafComponent c => c.Component.CN,
            _ => throw new NotImplementedException(),
        });

    public static DelegateColumn<IComponentContent> TopMostPortName()
        => new DelegateColumn<IComponentContent>(
            "TopMostPort",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent { Property : ICDTableContentProperty prop} p => prop.Port.GetTopMostUser().Name,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<IComponentContent> MostRelevantPortName()
        => new DelegateColumn<IComponentContent>(
            "PortEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent { Property: ICDTableContentProperty prop } p => prop.Port.GetTopMostExposition().Name,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<IComponentContent> MostRelevantPortName_Regard()
        => new DelegateColumn<IComponentContent>(
            "ColEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent { Property: ICDTableContentProperty prop } p => "",
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<IComponentContent> SelfPortName()
        => new DelegateColumn<IComponentContent>(
            "PortSelf",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent { Property: ICDTableContentProperty prop } p => prop.Port.Name,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });
}
