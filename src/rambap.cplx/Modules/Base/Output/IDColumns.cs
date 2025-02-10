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

    // PN is displayed as exact

    public static DelegateColumn<IComponentContent> PartNumber()
        => new DelegateColumn<IComponentContent>("PN", ColumnTypeHint.StringExact,
             i => i.Component.Instance.PN,
             i => "TOTAL");

    // CN are used as pretty / common name, they are formated

    public static DelegateColumn<IComponentContent> ComponentNumber()
        => new DelegateColumn<IComponentContent>("CN", ColumnTypeHint.StringFormatable,
            i => i.Component.CN);

    public static DelegateColumn<IComponentContent> GroupCNs(int maxColumnWidth = 50)
       => new DelegateColumn<IComponentContent>("Component CNs", ColumnTypeHint.StringFormatable,
            i =>
            {
                var componentCNs = i.AllComponents().Select(c => c.component.CN);
                return maxColumnWidth switch
                {
                    > 0 => JoinWithMaxLength(", ", componentCNs, maxColumnWidth),
                    _ => string.Join(", ", componentCNs),
                };
            });

    // CID as displayed as non formatable in order to prevent adding spaces in a path like string

    public static DelegateColumn<IComponentContent> ComponentID()
        => new DelegateColumn<IComponentContent>("CID", ColumnTypeHint.StringExact,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                return Core.CID.RemoveImplicitRoot(CID);
            },
            i => "TOTAL"
            );

    public static DelegateColumn<IComponentContent> GroupCIDs(int maxColumnWidth = 50)
        => new DelegateColumn<IComponentContent>("Component CIDs", ColumnTypeHint.StringExact,
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

    public static DelegateColumn<IComponentContent> ComponentID_And_Property(string propname) =>
        new DelegateColumn<IComponentContent>("CID", ColumnTypeHint.StringExact,
            i =>
            {
                var CID = Core.CID.Append(i.Location.CIN, i.Component.CN);
                CID = Core.CID.RemoveImplicitRoot(CID);
                return i switch
                {
                    IPropertyContent p => CID + Core.CID.Separator + propname,
                    _ => CID
                };
            });

    
    public static IColumn<IComponentContent> ComponentNumberPrettyTree(Func<IPropertyContent, string>? propertyNaming = null)
        => new ComponentPrettyTreeColumn()
        {
            Title = "CN",
            GetLocationText = i =>
            {
                var componentOrPartGroupName = i.IsGrouping
                        ? $"{i.ComponentLocalCount}x: {i.Component.Instance.PN}" // Group present the "n x PN"
                        : $"{i.Component.CN}";// Single components present the CN
                if(i is IPropertyContent pc)
                {
                    var propName = propertyNaming?.Invoke(pc) ?? "?";
                    var shouldStillDisplayPN = pc.IsLeafBecause == LeafCause.SingleStackedPropertyChild;
                    return shouldStillDisplayPN
                        ? $"{componentOrPartGroupName} / {propName}"
                        : $"/ {propName}";
                }
                else
                    return componentOrPartGroupName;
                
            }
        };

    public static DelegateColumn<IComponentContent> ContentLocation()
        => new DelegateColumn<IComponentContent>("Location", ColumnTypeHint.StringFormatable,
            i =>
            {
                var loc = i.Location;
                return $"dep{loc.Depth} - {loc.LocalItemIndex+1} of {loc.LocalItemCount}";
            });
}
