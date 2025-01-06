using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rambap.cplx.Modules.Base.Output.CommonColumns;

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

    
    public static IColumn<ComponentContent> ComponentNumberPrettyTree()
        => new ComponentPrettyTreeColumn()
        {
            Title = "CN",
            GetLocationText = i =>
                i is LeafProperty
                    ? $"/" // This is a property
                    : i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.PN}" // Group present the "n x PN"
                        : $"{i.Component.CN}" // Single components present the CN
        };

    public static DelegateColumn<ComponentContent> ContentLocation()
        => new DelegateColumn<ComponentContent>("Location", ColumnTypeHint.String,
            i =>
            {
                var loc = i.Location;
                return $"dep{loc.Depth} - {loc.LocalItemIndex+1} of {loc.LocalItemCount}";
            });
}
