using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class PartTreeCommons
{
    public static IColumn<PartTtreeItem> LineNumber()
        => new LineNumberColumn<PartTtreeItem>();
    public static IColumn<PartTtreeItem> GroupNumber()
        => new LineNumberColumnWithContinuation<PartTtreeItem>()
        { ContinuationCondition = (i, j) => i == null || i.Items != j.Items };

    public static DelegateColumn<PartTtreeItem> GroupPN() =>
        new DelegateColumn<PartTtreeItem>("PN", ColumnTypeHint.String,
            i => i.PrimaryItem.Component.Instance.PN,
            i => "TOTAL");

    public static DelegateColumn<PartTtreeItem> GroupCNs() =>
        new DelegateColumn<PartTtreeItem>("Component IDs", ColumnTypeHint.String,
            i =>
            {
                var componentCNs = i.Items
                    .Select(c => CID.Append(c.Location.CIN, c.Component.CN))
                    .Select(s => CID.RemoveImplicitRoot(s));

                return string.Join(", ", componentCNs);
            });

    public static DelegateColumn<PartTtreeItem> GroupCount() =>
        new DelegateColumn<PartTtreeItem>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                return i.Items.Count().ToString();
            });
}

