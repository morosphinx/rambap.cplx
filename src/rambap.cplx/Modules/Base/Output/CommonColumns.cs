using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Base.Output;

public static class CommonColumns
{
    public static DelegateColumn<ICplxContent> Dashes(string title)
        => new DelegateColumn<ICplxContent>(
            title,
            ColumnTypeHint.StringExact,
            i => new string('-', title.Length));

    public static IColumn<ICplxContent> EmptyColumn(string title = "")
        => new DelegateColumn<ICplxContent>(title, ColumnTypeHint.StringFormatable,
            i => "");

    public static IColumn<ICplxContent> LineNumber()
        => new LineNumberColumn<ICplxContent>();

    public static IColumn<ICplxContent> LineTypeNumber()
        => new LineNumberColumnWithContinuation<ICplxContent>()
            { ContinuationCondition = (i, j) => i == null || i.Component != j.Component };

    public static DelegateColumn<ICplxContent> ComponentDepth()
        => new DelegateColumn<ICplxContent>("Depth", ColumnTypeHint.Numeric,
            i => i.Location.Depth.ToString());

    public static DelegateColumn<ICplxContent> ComponentTotalCount(bool displayBranches = false)
        => new DelegateColumn<ICplxContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                BranchComponent bc when !displayBranches => "",
                _ => i.ComponentTotalCount.ToString(),
            });

    public static DelegateColumn<ICplxContent> ComponentComment() =>
        new DelegateColumn<ICplxContent>("Component description", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                // In case of a group of component, only display if the components have the same comment
                var con when con.IsGrouping => con.AllComponentsMatch(c => c.Comment, out var val) ? val : "", 
                _ => i.Component.Comment,
            });


    public class ComponentPrettyTreeColumn : IColumn<ICplxContent>
    {
        public required string Title { get; set; }
        public bool CanFormat = false;
        public ColumnTypeHint TypeHint =>
            CanFormat ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact ;

        private List<bool> LevelDone { get; } = [];
        public string CellFor(ICplxContent item)
        {
            while (LevelDone.Count <= item.Location.Depth) LevelDone.Add(false);
            LevelDone[item.Location.Depth] = false;
            //
            string ver = " │ "; // That's an Alt+179, and not an Alt+124 '|', this latter is reserved for markdown 
            bool isEnd = item.Location.IsEnd; //item.Location.LocalItemIndex == item.Location.LocalItemCount - 1;
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

        public required Func<ICplxContent, string> GetLocationText { get; init; }
    }
}


