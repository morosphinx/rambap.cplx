using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Base.Output;

public static class PartContentColumns
{
    public static IColumn<ComponentContent> MainComponentInfo(IColumn<ComponentContent> componentColumn)
        => new DelegateColumn<ComponentContent>(componentColumn.Title, componentColumn.TypeHint,
            componentColumn.CellFor);

    public static IColumn<ComponentContent> EmptyColumn(string title = "")
        => new DelegateColumn<ComponentContent>(title, ColumnTypeHint.String,
            i => "");

    public static IColumn<ComponentContent> LineNumber()
        => new LineNumberColumn<ComponentContent>();

    public static IColumn<ComponentContent> GroupNumber()
        => new LineNumberColumnWithContinuation<ComponentContent>()
        { ContinuationCondition = (i, j) => i == null || i.Component != j.Component };

    public static DelegateColumn<ComponentContent> GroupPN() =>
        new DelegateColumn<ComponentContent>("PN", ColumnTypeHint.String,
            i => i.Component.Instance.PN,
            i => "TOTAL");

    public static DelegateColumn<ComponentContent> GroupCNs() =>
        new DelegateColumn<ComponentContent>("Component IDs", ColumnTypeHint.String,
            i =>
            {
                var componentCNs = i.AllComponents()
                    .Select(c => CID.Append(c.location.CIN, c.component.CN))
                    .Select(s => CID.RemoveImplicitRoot(s));

                return string.Join(", ", componentCNs);
            });

    public static DelegateColumn<ComponentContent> GroupCount() =>
        new DelegateColumn<ComponentContent>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                return i.ComponentCount.ToString();
            });
}

