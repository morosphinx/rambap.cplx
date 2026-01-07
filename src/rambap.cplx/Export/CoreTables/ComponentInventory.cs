using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Documentation.Outputs;
using System.Diagnostics.CodeAnalysis;
using rambap.cplx.Instantiation;

namespace rambap.cplx.Export.CoreTables;

public record class ComponentInventory : TableProducer<IContent>
{
    [SetsRequiredMembers]
    public ComponentInventory(DocumentationPerimeter? perimeter = null)
    {
        Iterator = new PartTypesIterator<object>()
        {
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

