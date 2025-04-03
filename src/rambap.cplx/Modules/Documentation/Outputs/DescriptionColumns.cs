using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    public static DelegateColumn<ICplxContent> PartDescription(bool allLines = false) =>
        new DelegateColumn<ICplxContent>("Part Description", ColumnTypeHint.StringFormatable,
            i => allLines
                ? i.Component.Instance.Documentation()?.GetAllLineDescription() ?? ""
                : i.Component.Instance.Documentation()?.GetSingleLineDescription() ?? "");

    public static DelegateColumn<ICplxContent> PartCommonName(bool hideIfEqualPN =false) =>
        new DelegateColumn<ICplxContent>("Part Common Name", ColumnTypeHint.StringFormatable,
            i =>
            {
                var instance = i.Component.Instance;
                if(hideIfEqualPN)
                    return instance.CommonName == instance.PN ? "" : instance.CommonName;
                else return instance.CommonName;
            });

    public static DelegateColumn<ICplxContent> PartLink() =>
        new DelegateColumn<ICplxContent>("Link", ColumnTypeHint.StringExact,
            i => i.Component.Instance.Documentation()?.Links.FirstOrDefault()?.Text ?? "");
}

