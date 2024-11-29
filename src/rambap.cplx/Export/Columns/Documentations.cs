using rambap.cplx;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class Documentations
{
    public static DelegateColumn<PartTreeItem> GroupDescription() =>
        new DelegateColumn<PartTreeItem>("Description", ColumnTypeHint.String,
            i =>
            {
                var descriptions = i.PrimaryItem.Component.Instance.Descriptions()?.Descriptions.Select(d => d.Text)
                    ?? new List<string>();
                return string.Join(" ", descriptions);
            });

    public static DelegateColumn<ComponentTreeItem> PartDescription() =>
        new DelegateColumn<ComponentTreeItem>("Part description", ColumnTypeHint.String,
            i => i.Component.Instance.Descriptions()?.Descriptions.First().Text);
}

