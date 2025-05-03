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

    public static DelegateColumn<ICplxContent> PartLink() =>
        new DelegateColumn<ICplxContent>("Link", ColumnTypeHint.StringExact,
            i => i.Component.Instance.Documentation()?.Links.FirstOrDefault()?.Text ?? "");
}

