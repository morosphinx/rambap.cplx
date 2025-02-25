using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using static rambap.cplx.Modules.Connectivity.Outputs.ICDTableIterator;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    // Port names are displayed as exact strings, no formating

    public static DelegateColumn<ICplxContent> TopMostPortPart()
    => new DelegateColumn<ICplxContent>(
        "Part",
        ColumnTypeHint.StringExact,
        i => i switch
        {
            BranchComponent b => b.Component.CN,
            IPropertyContent<ICDTableContentProperty> p => p.Property.Port.GetTopMostUser().Owner.CN ?? "",
            LeafComponent c => c.Component.CN,
            _ => throw new NotImplementedException(),
        });

    public static DelegateColumn<ICplxContent> TopMostPortName()
        => new DelegateColumn<ICplxContent>(
            "TopMostPort",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent<ICDTableContentProperty> p => p.Property.Port.GetTopMostUser().Label,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> MostRelevantPortName()
        => new DelegateColumn<ICplxContent>(
            "PortEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent<ICDTableContentProperty> p => p.Property.Port.GetTopMostExposition().Label,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> MostRelevantPortName_Regard()
        => new DelegateColumn<ICplxContent>(
            "ColEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent<ICDTableContentProperty> p => "",
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> SelfPortName()
        => new DelegateColumn<ICplxContent>(
            "PortSelf",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                BranchComponent b => "",
                IPropertyContent<ICDTableContentProperty> p => p.Property.Port.Label,
                LeafComponent c => "",
                _ => throw new NotImplementedException(),
            });
}
