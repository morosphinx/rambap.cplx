using rambap.cplx.Core;
using rambap.cplx.Export;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Base.Output;

public static class ComponentContentColumns
{

    public static IColumn<ComponentContent> EmptyColumn(string title = "")
        => new DelegateColumn<ComponentContent>(title, ColumnTypeHint.String,
            i => "");

    public static IColumn<ComponentContent> LineNumber()
        => new LineNumberColumn<ComponentContent>();

    public static DelegateColumn<ComponentContent> ComponentDepth()
        => new DelegateColumn<ComponentContent>("Depth", ColumnTypeHint.Numeric,
            i => i.Location.Depth.ToString());

    public static DelegateColumn<ComponentContent> ComponentID() =>
        new DelegateColumn<ComponentContent>("CID", ColumnTypeHint.String,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                return Core.CID.RemoveImplicitRoot(CID);
            },
            i => "TOTAL"
            );

    private class ComponentPrettyTreeColumn : IColumn<ComponentContent>
    {
        public string Title => "CN";
        public ColumnTypeHint TypeHint => ColumnTypeHint.String;

        private List<bool> LevelDone { get; } = [];
        public string CellFor(ComponentContent item)
        {
            if (LevelDone.Count <= item.Location.Depth) LevelDone.Add(false);
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

    public static IColumn<ComponentContent> ComponentPrettyTree() =>
        new ComponentPrettyTreeColumn();

    public static DelegateColumn<ComponentContent> ComponentID_And_Property(string propname) =>
        new DelegateColumn<ComponentContent>("CID", ColumnTypeHint.String,
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

    public static DelegateColumn<ComponentContent> ComponentNumber() =>
        new DelegateColumn<ComponentContent>("CN", ColumnTypeHint.String,
            i => i.Component.CN);

    public static DelegateColumn<ComponentContent> PartNumber() =>
         new DelegateColumn<ComponentContent>("PN", ColumnTypeHint.String,
             i => i.Component.Instance.PN);

    public static DelegateColumn<ComponentContent> ComponentComment() =>
        new DelegateColumn<ComponentContent>("Component description", ColumnTypeHint.String,
            i => i.Component.Comment);
}


