using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class ComponentTreeCommons
{
    public static IColumn<ComponentTreeItem> LineNumber()
        => new LineNumberColumn<ComponentTreeItem>();

    public static DelegateColumn<ComponentTreeItem> ComponentDepth() =>
    new DelegateColumn<ComponentTreeItem>("Depth", ColumnTypeHint.Numeric,
        i => i.Location.Depth.ToString());

    public static DelegateColumn<ComponentTreeItem> ComponentID() =>
        new DelegateColumn<ComponentTreeItem>("CID", ColumnTypeHint.String,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                return Core.CID.RemoveImplicitRoot(CID);
            },
            i => "TOTAL"
            );

    private class ComponentPrettyTreeColumn : IColumn<ComponentTreeItem>
    {
        public string Title => "CN";
        public ColumnTypeHint TypeHint => ColumnTypeHint.String;

        private List<bool> LevelDone { get; } = new List<bool>();
        public string CellFor(ComponentTreeItem item)
        {
            if (LevelDone.Count() <= item.Location.Depth) LevelDone.Add(false);
            LevelDone[item.Location.Depth] = false;

            string ver = " │ "; // That's an Alt+179, and not an Alt+124 '|', this latter is reserved for markdown 
            bool isEnd = item.Location.ComponentIndex == item.Location.ComponentCount - 1;
            string end = isEnd ? " └─" : " ├─";
            if (item.Location.Depth > 0)
                LevelDone[item.Location.Depth - 1] = isEnd;

            int ver_ctn = Math.Max(item.Location.Depth - 1, 0);
            int end_cent = Math.Min(item.Location.Depth, 1);
            List<string> strs = [
                .. Enumerable.Range(0, ver_ctn).Select(i => LevelDone[i] ? "   " : ver ),
                .. Enumerable.Range(0, end_cent).Select(i => end),
                " ",
                item.Component.CN,
            ];
            return string.Concat(strs);
        }

        public void Reset() => LevelDone.Clear();
        public string TotalFor(Pinstance root) => "";
    }

    public static IColumn<ComponentTreeItem> ComponentPrettyTree() =>
        new ComponentPrettyTreeColumn();

    public static DelegateColumn<ComponentTreeItem> ComponentID_And_Property(string propname) =>
        new DelegateColumn<ComponentTreeItem>("CID", ColumnTypeHint.String,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                CID = Core.CID.RemoveImplicitRoot(CID);
                if (i is LeafProperty prop)
                {
                    var propSuffix = "/::" + propname;
                    return CID + propSuffix;
                }
                else
                    return CID;
            });

    public static DelegateColumn<ComponentTreeItem> ComponentNumber() =>
        new DelegateColumn<ComponentTreeItem>("CN", ColumnTypeHint.String,
            i => i.Component.CN);

    public static DelegateColumn<ComponentTreeItem> PartNumber() =>
         new DelegateColumn<ComponentTreeItem>("PN", ColumnTypeHint.String,
             i => i.Component.Instance.PN);

    public static DelegateColumn<ComponentTreeItem> ComponentComment() =>
        new DelegateColumn<ComponentTreeItem>("Component description", ColumnTypeHint.String,
            i => i.Component.Comment);
}


