﻿using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class SystemViewTables
{
    public static TableProducer<ICplxContent> ComponentTree_Detailled()
        => new()
        {
            Iterator = new ComponentIterator(),
            Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                IDColumns.PartNumber(),
                CommonColumns.ComponentComment(),
                DescriptionColumns.PartDescription(),
            ],
        };

    public static TableProducer<ICplxContent> ComponentTree_Stacked()
       => new()
       {
           Iterator = new ComponentIterator()
           {
               GroupPNsAtSameLocation = true,
           },
           Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                CommonColumns.ComponentTotalCount(displayBranches : true),
                IDColumns.GroupCNs(),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
           ],
       };

    public static TableProducer<ICplxContent> ComponentInventory()
        => new()
        {
            Iterator = new PartTypesIterator<object>()
            {
                WriteBranches = true,
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                DescriptionColumns.PartCommonName(hideIfEqualPN : true),
                IDColumns.PartNumber(),
                DescriptionColumns.PartDescription(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.GroupCIDs(),
                DescriptionColumns.PartLink(),
            ],
        };

    public static TableProducer<ICplxContent> BillOfMaterials()
    => new()
    {
        Iterator = new PartTypesIterator<object>()
        {
            WriteBranches = true,
        },
        Columns = [
            CommonColumns.LineTypeNumber(),
            IDColumns.PartNumber(),
            DescriptionColumns.PartCommonName(hideIfEqualPN : true),
            CommonColumns.ComponentTotalCount(),
            SupplyChain.Outputs.SupplierColumns.SupplierName(),
            SupplyChain.Outputs.SupplierColumns.SupplierPN(),
            SupplyChain.Outputs.SupplierColumns.SupplierLink(),
        ],
    };
}

