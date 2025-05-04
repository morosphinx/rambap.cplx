using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

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
                IPureComponentContent c => c.Component.CN,
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperUsage().Owner.Parent.CN ?? "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> TopMostPortName()
        => new DelegateColumn<ICplxContent>(
            "TopMostPort",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPureComponentContent c => "",
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperUsage().Label,
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> MostRelevantPortName()
        => new DelegateColumn<ICplxContent>(
            "PortEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPureComponentContent c => "",
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperExposition().Label,
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> MostRelevantPortName_Regard()
        => new DelegateColumn<ICplxContent>(
            "ColEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPureComponentContent c => "",
                IPropertyContent<ICDTableProperty> p => "",
                _ => throw new NotImplementedException(),
            });

    public static DelegateColumn<ICplxContent> SelfPortName()
        => new DelegateColumn<ICplxContent>(
            "PortSelf",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPureComponentContent c => "",
                IPropertyContent<ICDTableProperty> p => p.Property.Port.Label,
                _ => throw new NotImplementedException(),
            });
}
