using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Tables;
using rambap.cplx.Core;

namespace rambap.cplx.Modules.Costing.Outputs;

public static partial class CostTables
{
    /// <summary>
    /// Enumerate the instance native costs, OR return a single 0 if there is no cost.
    /// Used to display costless part in bill of materials
    /// We want to be aware if something cost 0.0 in those
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static IEnumerable<InstanceCost.NativeCostInfo> ListCostOr0(Core.Component c)
    {
        if (c.Instance.Cost() is not null and var cost)
        {
            var nativeCosts = cost.NativeCosts;
            if (nativeCosts.Count == 0)
                return [new InstanceCost.NativeCostInfo("", 0)];
            else
                return nativeCosts;
        }
        else
            return [new InstanceCost.NativeCostInfo("", 0)];
    }

    /// <summary>
    /// Table detailing the amount and duration of each individual Cost of the instance.
    /// </summary>
    public static TableProducer<ICplxContent> CostBreakdown()
        => new()
        {
            Iterator = new ComponentPropertyIterator<InstanceCost.NativeCostInfo>()
            {
                PropertyIterator = (c) => c.Instance.Cost()?.NativeCosts.AsEnumerable() ?? [],
                GroupPNsAtSameLocation = true,
                StackPropertiesSingleChildBranches = true,
            },
            Columns = [
                IDColumns.ComponentNumberPrettyTree<InstanceCost.NativeCostInfo>(pc => pc.Property.name),
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

