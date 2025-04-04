﻿using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Costing.Outputs;

public static class CostTables
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
    /// Table listing the amount and cost of each part kind in the instance
    /// </summary>
    /// <param name="recurse">If true, the entire component tree is returned. <br/>
    /// If false, only the immediate components are returned.</param>
    public static TableProducer<ICplxContent> BillOfMaterial(bool recurse = true)
        => new()
        {
            Iterator = new PartTypesIterator<InstanceCost.NativeCostInfo>()
            {
                WriteBranches = false,
                RecursionCondition = recurse ? null : (c, l) => false, // null = always recurse
                PropertyIterator = ListCostOr0
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
                CostColumns.CostName(),
                CostColumns.UnitCost(),
                CommonColumns.ComponentTotalCount(),
                CostColumns.TotalCost(),
            ],
        };

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

