using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Base.Output;

public static class PartContentColumns
{
    public static IColumn<PartContent> EmptyColumn(string title = "")
        => new DelegateColumn<PartContent>(title, ColumnTypeHint.String,
            i => "");

    public static IColumn<PartContent> LineNumber()
        => new LineNumberColumn<PartContent>();
    public static IColumn<PartContent> GroupNumber()
        => new LineNumberColumnWithContinuation<PartContent>()
        { ContinuationCondition = (i, j) => i == null || i.Items != j.Items };

    public static DelegateColumn<PartContent> GroupPN() =>
        new DelegateColumn<PartContent>("PN", ColumnTypeHint.String,
            i => i.PrimaryItem.Component.Instance.PN,
            i => "TOTAL");

    public static DelegateColumn<PartContent> GroupCNs() =>
        new DelegateColumn<PartContent>("Component IDs", ColumnTypeHint.String,
            i =>
            {
                var componentCNs = i.Items
                    .Select(c => CID.Append(c.Location.CIN, c.Component.CN))
                    .Select(s => CID.RemoveImplicitRoot(s));

                return string.Join(", ", componentCNs);
            });

    public static DelegateColumn<PartContent> GroupCount() =>
        new DelegateColumn<PartContent>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                return i.Items.Count().ToString();
            });
}

