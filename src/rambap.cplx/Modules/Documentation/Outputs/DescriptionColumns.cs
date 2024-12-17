using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class DescriptionColumns
{
    private static string GetDescription(Pinstance instance)
    {
        var descriptions = instance.Descriptions()?.Descriptions.Select(d => d.Text)
                    ?? new List<string>();
        return string.Join(" ", descriptions);
    }

    public static DelegateColumn<PartContent> GroupDescription() =>
        new DelegateColumn<PartContent>("Description", ColumnTypeHint.String,
            i => GetDescription(i.PrimaryItem.Component.Instance)
            );

    public static DelegateColumn<ComponentContent> PartDescription() =>
        new DelegateColumn<ComponentContent>("Part description", ColumnTypeHint.String,
            i => GetDescription(i.Component.Instance));
}

