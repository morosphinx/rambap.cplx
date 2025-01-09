using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    private static string GetDescription(Pinstance instance)
    {
        var descriptions = instance.Documentation()?.Descriptions.Select(d => d.Text)
                    ?? new List<string>();
        return string.Join(" ", descriptions);
    }

    public static DelegateColumn<IComponentContent> GroupDescription() =>
        new DelegateColumn<IComponentContent>("Description", ColumnTypeHint.String,
            i => GetDescription(i.Component.Instance)
            );

    public static DelegateColumn<IComponentContent> PartDescription() =>
        new DelegateColumn<IComponentContent>("Part description", ColumnTypeHint.String,
            i => GetDescription(i.Component.Instance));
}

