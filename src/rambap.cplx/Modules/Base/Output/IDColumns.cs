using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.Modules.Base.Output;

public static class IDColumns
{
    private static string JoinWithMaxLength(string delimiter, IEnumerable<string> strings, int maxLength)
    {
        List<string> ReturnUntilLengthPassed(IEnumerable<string> strings, int maxLength, out bool allPassed)
        {
            allPassed = true;
            int passedLength = 0;
            List<string> passedStrings = [];
            foreach (var s in strings)
            {
                if (passedLength > maxLength)
                {
                    allPassed = false;
                    break;
                }
                else
                {
                    passedLength += s.Length;
                    passedStrings.Add(s);
                }
            }
            return passedStrings;
        }
        bool allDisplayed = false;
        var displayedStrings = ReturnUntilLengthPassed(strings, maxLength, out allDisplayed);
        int totalStringCount = strings.Count();
        int nonDisplayedStringCount = totalStringCount - displayedStrings.Count();
        // Add a reminder if not all strings are displayed
        if (!allDisplayed)
            displayedStrings.Add($"... [{nonDisplayedStringCount} more]");
        return string.Join(delimiter, displayedStrings);
    }

    public static DelegateColumn<ComponentContent> PartNumber()
        => new DelegateColumn<ComponentContent>("PN", ColumnTypeHint.String,
             i => i.Component.Instance.PN,
             i => "TOTAL");

    public static DelegateColumn<ComponentContent> ComponentNumber()
        => new DelegateColumn<ComponentContent>("CN", ColumnTypeHint.String,
            i => i.Component.CN);

    public static DelegateColumn<ComponentContent> GroupCNs(int maxColumnWidth = 50)
       => new DelegateColumn<ComponentContent>("Component CNs", ColumnTypeHint.String,
            i =>
            {
                var componentCNs = i.AllComponents().Select(c => c.component.CN);
                return maxColumnWidth switch
                {
                    > 0 => JoinWithMaxLength(", ", componentCNs, maxColumnWidth),
                    _ => string.Join(", ", componentCNs),
                };
            });

    public static DelegateColumn<ComponentContent> ComponentID()
        => new DelegateColumn<ComponentContent>("CID", ColumnTypeHint.String,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                return Core.CID.RemoveImplicitRoot(CID);
            },
            i => "TOTAL"
            );

    public static DelegateColumn<ComponentContent> GroupCIDs(int maxColumnWidth = 50)
        => new DelegateColumn<ComponentContent>("Component CIDs", ColumnTypeHint.String,
            i =>
            {
                var componentCIDs = i.AllComponents()
                    .Select(c => CID.Append(c.location.CIN, c.component.CN))
                    .Select(s => CID.RemoveImplicitRoot(s));
                return maxColumnWidth switch
                {
                    > 0 => JoinWithMaxLength(", ", componentCIDs, maxColumnWidth),
                    _ => string.Join(", ", componentCIDs),
                };
            });

    public static DelegateColumn<ComponentContent> ComponentID_And_Property(string propname) =>
        new DelegateColumn<ComponentContent>("CID", ColumnTypeHint.String,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                CID = Core.CID.RemoveImplicitRoot(CID);
                return i switch
                {
                    LeafProperty p => CID + "/::" + propname,
                    _ => CID
                };
            });

    private class ComponentNumberPrettyTreeColumn : IColumn<ComponentContent>
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

            bool isGrouping = item.ComponentLocalCount > 1;

            bool isLeafProperty = item is LeafProperty;
            var effectiveDepth = item.Location.Depth + (isLeafProperty ? 1 : 0);
            // TODO : isEnd value is incorrect when adding a virtual level on isLeafProperty
            // => Incorrectly show " ├─" even if no successor

            int ver_ctn = Math.Max(effectiveDepth - 1, 0);
            int end_cent = Math.Min(effectiveDepth, 1);
            List<string> strs = [
                .. Enumerable.Range(0, ver_ctn).Select(i => LevelDone[i] ? "   " : ver ),
                .. Enumerable.Range(0, end_cent).Select(i => end),
                " ",
                isLeafProperty
                    ? $"*" // This is a property
                    : isGrouping
                        ? $"{item.ComponentLocalCount}x: {item.Component.Instance.PN}"
                        : $"{item.Component.CN}",
            ];
            return string.Concat(strs);
        }

        public void Reset() => LevelDone.Clear();
        public string TotalFor(Pinstance root) => "";
    }
    public static IColumn<ComponentContent> ComponentNumberPrettyTree()
        => new ComponentNumberPrettyTreeColumn();
}
