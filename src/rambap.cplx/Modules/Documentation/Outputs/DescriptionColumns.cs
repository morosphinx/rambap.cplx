using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{


    public static DelegateColumn<IComponentContent> PartDescription(bool allLines = false) =>
        new DelegateColumn<IComponentContent>("Part Description", ColumnTypeHint.StringFormatable,
            i => allLines
                ? i.Component.Instance.Documentation()?.GetAllLineDescription() ?? ""
                : i.Component.Instance.Documentation()?.GetSingleLineDescription() ?? "");

    public static DelegateColumn<IComponentContent> PartCommonName(bool hideIfEqualPN =false) =>
        new DelegateColumn<IComponentContent>("Part Common Name", ColumnTypeHint.StringFormatable,
            i =>
            {
                var instance = i.Component.Instance;
                if(hideIfEqualPN)
                    return instance.CommonName == instance.PN ? "" : instance.CommonName;
                else return instance.CommonName;
            });

    public static DelegateColumn<IComponentContent> PartLink() =>
        new DelegateColumn<IComponentContent>("Link", ColumnTypeHint.StringExact,
            i => i.Component.Instance.Documentation()?.Links.FirstOrDefault()?.Text ?? "");
}

