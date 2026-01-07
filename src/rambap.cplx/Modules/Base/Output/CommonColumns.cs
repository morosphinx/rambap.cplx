using rambap.cplx.Instantiation;
using rambap.cplx.Modules.Base.TableModel;

namespace rambap.cplx.Modules.Base.Output;

public static class CommonColumns
{
    public static DelegateColumn<IContent> Dashes(string title)
        => new DelegateColumn<IContent>(
            title,
            ColumnTypeHint.StringExact,
            i => new string('-', title.Length));

    public static IColumn<IContent> EmptyColumn(string title = "")
        => new DelegateColumn<IContent>(title, ColumnTypeHint.StringFormatable,
            i => "");

    public static IColumn<IContent> LineNumber()
        => new LineNumberColumn<IContent>();

    public static IColumn<IContent> LineTypeNumber()
        => new LineNumberColumnWithContinuation<IContent>()
            { ContinuationCondition = (i, j) => i == null || i.Component != j.Component };

    public static DelegateColumn<IContent> ComponentDepth()
        => new DelegateColumn<IContent>("Depth", ColumnTypeHint.Numeric,
            i => i.Location.Depth.ToString());

    public static DelegateColumn<IContent> ComponentTotalCount(bool displayBranches = false)
        => new DelegateColumn<IContent>("Count", ColumnTypeHint.Numeric,
            i => i switch
            {
                _ when i.IsBranch && !displayBranches => "",
                _ => i.ComponentTotalCount.ToString(),
            });

    public static DelegateColumn<IContent> ComponentComment() =>
        new DelegateColumn<IContent>("Component description", ColumnTypeHint.StringFormatable,
            i => i switch
            {
                // In case of a group of component, only display if the components have the same comment
                var con when con.IsGrouping => con.AllComponentsMatch(c => c.Comment, out var val) ? val : "", 
                _ => i.Component.Comment,
            });


    public class ComponentPrettyTreeColumn : IColumn<IContent>
    {
        public required string Title { get; set; }
        public bool CanFormat = false;
        public ColumnTypeHint TypeHint =>
            CanFormat ? ColumnTypeHint.StringFormatable : ColumnTypeHint.StringExact ;

        private List<bool> LevelDone { get; } = [];
        public string CellFor(IContent item)
        {
            while (LevelDone.Count <= item.Location.Depth) LevelDone.Add(false);
            LevelDone[item.Location.Depth] = false;
            //
            string ver = " │ "; // That's an Alt+179, and not an Alt+124 '|', this latter is reserved for markdown 
            bool isEnd = item.LocationWhenFlattened?.IsEnd ?? false; //item.Location.LocalItemIndex == item.Location.LocalItemCount - 1;
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

        public required Func<IContent, string> GetLocationText { get; init; }
    }
}


