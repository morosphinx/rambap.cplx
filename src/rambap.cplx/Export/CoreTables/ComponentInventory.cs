using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

public record class ComponentInventory : TableProducer<ICplxContent>
{
    [SetsRequiredMembers]
    public ComponentInventory(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new PartTypesIterator<object>()
        {
            WriteBranches = true,
            DocumentationPerimeter = perimeter ?? new(),
        };
        Columns = [
            CommonColumns.LineTypeNumber(),
            IDColumns.PartCommonName(usePnAsBackup : false),
            IDColumns.PartNumber(),
            DescriptionColumns.PartDescription(),
            CommonColumns.ComponentTotalCount(),
            IDColumns.GroupCIDs(),
            DescriptionColumns.PartLink(),
        ];
    }
}

