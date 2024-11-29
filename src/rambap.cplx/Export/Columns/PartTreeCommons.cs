using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class PartTreeCommons
{
    public static IColumn<PartTreeItem> LineNumber()
        => new LineNumberColumn<PartTreeItem>();
    public static IColumn<PartTreeItem> GroupNumber()
        => new LineNumberColumnWithContinuation<PartTreeItem>()
        { ContinuationCondition = (i, j) => i == null || i.Items != j.Items };

    public static DelegateColumn<PartTreeItem> GroupPN() =>
        new DelegateColumn<PartTreeItem>("PN", ColumnTypeHint.String,
            i => i.PrimaryItem.Component.Instance.PN,
            i => "TOTAL");

    public static DelegateColumn<PartTreeItem> GroupCNs() =>
        new DelegateColumn<PartTreeItem>("Component IDs", ColumnTypeHint.String,
            i =>
            {
                var componentCNs = i.Items
                    .Select(c => CID.Append(c.Location.CIN, c.Component.CN))
                    .Select(s => CID.RemoveImplicitRoot(s));

                return string.Join(", ", componentCNs);
            });

    public static DelegateColumn<PartTreeItem> GroupCount() =>
        new DelegateColumn<PartTreeItem>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                return i.Items.Count().ToString();
            });
}

