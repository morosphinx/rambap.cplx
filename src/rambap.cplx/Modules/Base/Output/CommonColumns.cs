using rambap.cplx.Core;
using rambap.cplx.Export.Tables;

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

    public static DelegateColumn<ComponentContent> ComponentTotalCount(bool includeBranches = false)
        => new DelegateColumn<ComponentContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                BranchComponent bc when !includeBranches => "",
                _ => i.ComponentTotalCount.ToString(),
            });

    public static DelegateColumn<ComponentContent> ComponentComment() =>
        new DelegateColumn<ComponentContent>("Component description", ColumnTypeHint.String,
            i => i.Component.Comment);

    public class ComponentPrettyTreeColumn : IColumn<ComponentContent>
    {
        public required string Title { get; init; }
        public ColumnTypeHint TypeHint => ColumnTypeHint.String;

        private List<bool> LevelDone { get; } = [];
        public string CellFor(ComponentContent item)
        {
            while (LevelDone.Count <= item.Location.Depth) LevelDone.Add(false);
            LevelDone[item.Location.Depth] = false;
            //
            string ver = " │ "; // That's an Alt+179, and not an Alt+124 '|', this latter is reserved for markdown 
            bool isEnd = item.Location.LocalItemIndex == item.Location.LocalItemCount - 1;
            string end = isEnd ? " └─" : " ├─";
            if (item.Location.Depth > 0)
                LevelDone[item.Location.Depth - 1] = isEnd;
            var depth = item.Location.Depth;
            //
            int ver_ctn = Math.Max(depth - 1, 0);
            int end_cent = Math.Min(depth, 1);
            List<string> strs = [
                .. Enumerable.Range(0, ver_ctn).Select(i => LevelDone[i] ? "   " : ver ),
                .. Enumerable.Range(0, end_cent).Select(i => end),
                " ",
                GetLocationText(item),
            ];
            return string.Concat(strs);
        }

        public void Reset() => LevelDone.Clear();
        public string TotalFor(Pinstance root) => "";

        public required Func<ComponentContent, string> GetLocationText { get; init; }
    }
}


