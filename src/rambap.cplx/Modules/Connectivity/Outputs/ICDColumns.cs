using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDColumns
{
    // Port names are displayed as exact strings, no formating

    public static DelegateColumn<IContent> TopMostPortPart()
        => new DelegateColumn<IContent>(
            "Part",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperUsage().Owner.User.CN ?? "",
                IContent c => c.Component.CN,
            });

    public static DelegateColumn<IContent> TopMostPortName()
        => new DelegateColumn<IContent>(
            "TopMostPort",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperUsage().Label,
                IContent c => "",
            });

    public static DelegateColumn<IContent> MostRelevantPortName()
        => new DelegateColumn<IContent>(
            "PortEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ICDTableProperty> p => p.Property.Port.GetUpperExposition().Label,
                IContent c => "",
            });

    public static DelegateColumn<IContent> MostRelevantPortName_Regard()
        => new DelegateColumn<IContent>(
            "ColEXP",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ICDTableProperty> p => "",
                IContent c => "",
            });

    public static DelegateColumn<IContent> SelfPortName()
        => new DelegateColumn<IContent>(
            "PortSelf",
            ColumnTypeHint.StringExact,
            i => i switch
            {
                IPropertyContent<ICDTableProperty> p => p.Property.Port.Label,
                IContent c => "",
            });
}
