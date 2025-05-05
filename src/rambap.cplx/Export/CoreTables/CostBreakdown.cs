using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

/// <summary>
/// Table detailing the amount of each individual Cost of the instance.
/// </summary>
public record class CostBreakdown : TableProducer<ICplxContent>
{
    /// <summary>
    /// Enumerate a component costs point, 
    /// Optionaly, return a single 0 if there is no cost, in order to have display for all parts.
    /// </summary>
    internal static IEnumerable<InstanceCost.CostPoint> EnumerateCostPoints(Component c, bool havePlaceholder)
    {
        if (c.Instance.Cost() is not null and var cost)
        {
            var allCostPoints = cost.AllCostPoints();
            if(allCostPoints.Any())
                return allCostPoints;
        }
        if (havePlaceholder)
            return [new InstanceCost.NativeCost(0, "")];
        else
            return [];
    }

    [SetsRequiredMembers]
    public CostBreakdown()
    {
        Iterator = new ComponentPropertyIterator<InstanceCost.CostPoint>()
        {
            PropertyIterator = (c) => EnumerateCostPoints(c, false),
            GroupPNsAtSameLocation = true,
            StackPropertiesSingleChildBranches = true,
        };
        Columns = [
            IDColumns.ComponentNumberPrettyTree<InstanceCost.CostPoint>(pc => pc.Property.Name),
            CostColumns.LocalSumCost(),
            IDColumns.ComponentID(),
            IDColumns.PartNumber(),
            CostColumns.CostName(displayBranches : false),
            CostColumns.UnitCost(displayBranches : false),
            CommonColumns.ComponentTotalCount(),
            CostColumns.TotalCost(),
        ];
    }
}

