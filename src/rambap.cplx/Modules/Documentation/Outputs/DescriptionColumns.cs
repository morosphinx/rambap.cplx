using rambap.cplx.Core;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    public static DelegateColumn<IContent> PartDescription(bool allLines = false) =>
        new DelegateColumn<IContent>("Part Description", ColumnTypeHint.StringFormatable,
            i => allLines
                ? i.Component.Instance.Documentation()?.GetAllLineDescription() ?? ""
                : i.Component.Instance.Documentation()?.GetSingleLineDescription() ?? "");

    public static DelegateColumn<IContent> PartLink() =>
        new DelegateColumn<IContent>("Link", ColumnTypeHint.StringExact,
            i => i.Component.Instance.Documentation()?.Links.FirstOrDefault()?.Text ?? "");
}

