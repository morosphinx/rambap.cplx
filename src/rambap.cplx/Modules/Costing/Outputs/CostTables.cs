using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Tables;
using rambap.cplx.Core;

namespace rambap.cplx.Modules.Costing.Outputs;

public static partial class CostTables
{
    /// <summary>
    /// Enumerate a component costs point, 
    /// Optionaly, return a single 0 if there is no cost, in order to have display for all parts.
    /// </summary>
    private static IEnumerable<InstanceCost.CostPoint> EnumerateCostPoints(Core.Component c, bool havePlaceholder)
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

    /// <summary>
    /// Table detailing the amount and duration of each individual Cost of the instance.
    /// </summary>
    public static TableProducer<ICplxContent> CostBreakdown()
        => new()
        {
            Iterator = new ComponentPropertyIterator<InstanceCost.CostPoint>()
            {
                PropertyIterator = (c) => EnumerateCostPoints(c,false),
                GroupPNsAtSameLocation = true,
                StackPropertiesSingleChildBranches = true,
            },
            Columns = [
                IDColumns.ComponentNumberPrettyTree<InstanceCost.CostPoint>(pc => pc.Property.Name),
                CostColumns.LocalSumCost(),
                IDColumns.ComponentID(),
                IDColumns.PartNumber(),
                CostColumns.CostName(displayBranches : false),
                CostColumns.UnitCost(displayBranches : false),
                CommonColumns.ComponentTotalCount(),
                CostColumns.TotalCost(),
            ],
        };
}

