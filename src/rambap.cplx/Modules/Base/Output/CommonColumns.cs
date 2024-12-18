using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Base.Output;

public static class CommonColumns
{
    public static IColumn<ComponentContent> EmptyColumn(string title = "")
        => new DelegateColumn<ComponentContent>(title, ColumnTypeHint.String,
            i => "");

    public static IColumn<ComponentContent> LineNumber()
        => new LineNumberColumn<ComponentContent>();

    public static IColumn<ComponentContent> LineTypeNumber()
        => new LineNumberColumnWithContinuation<ComponentContent>()
        { ContinuationCondition = (i, j) => i == null || i.Component != j.Component };

    public static DelegateColumn<ComponentContent> ComponentDepth()
        => new DelegateColumn<ComponentContent>("Depth", ColumnTypeHint.Numeric,
            i => i.Location.Depth.ToString());

    public static DelegateColumn<ComponentContent> GroupCount()
        => new DelegateColumn<ComponentContent>("Count", ColumnTypeHint.Numeric,
            i =>
            {
                return i.ComponentCount.ToString();
            });

    public static DelegateColumn<ComponentContent> ComponentComment() =>
        new DelegateColumn<ComponentContent>("Component description", ColumnTypeHint.String,
            i => i.Component.Comment);
}


