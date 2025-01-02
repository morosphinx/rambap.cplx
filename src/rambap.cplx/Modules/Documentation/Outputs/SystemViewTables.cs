﻿using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Export.Iterators;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Modules.Documentation.Outputs;

public static class SystemViewTables
{
    public static TableProducer<ComponentContent> ComponentTree_Detailled()
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

    public static TableProducer<ComponentContent> ComponentTree_Stacked()
       => new()
       {
           Iterator = new ComponentIterator()
           {
               GroupPNsAtSameLocation = true,
           },
           Columns = [
                IDColumns.ComponentNumberPrettyTree(),
                CommonColumns.ComponentTotalCount(),
                IDColumns.GroupCNs(),
                IDColumns.PartNumber(),
                CommonColumns.ComponentComment(),
                DescriptionColumns.PartDescription(),
           ],
       };

    public static TableProducer<ComponentContent> ComponentInventory()
        => new()
        {
            Iterator = new PartTypesIterator()
            {
                WriteBranches = true,
            },
            Columns = [
                CommonColumns.LineTypeNumber(),
                IDColumns.PartNumber(),
                IDColumns.GroupCIDs(),
                DescriptionColumns.GroupDescription(),
                CommonColumns.ComponentTotalCount(),
            ],
        };
}

