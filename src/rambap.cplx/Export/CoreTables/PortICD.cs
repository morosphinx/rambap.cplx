using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Core;
using rambap.cplx.Modules.Base.TableModel;
using rambap.cplx.Modules.Connectivity.Outputs;
using System.Diagnostics.CodeAnalysis;

namespace rambap.cplx.Export.CoreTables;

public record class PortICD : TableProducer<ICplxContent>
{
    // ICD Table

    private static IEnumerable<ICDTableProperty> SelectPublicTopmostConnectors(Component component)
    {
        var connectivity = component.Instance.Connectivity();
        if (connectivity != null)
        {
            var publicConnectors = connectivity.Connectors.Where(c => c.IsPublic);
            var publicTopMostConnectors = publicConnectors.Where(c => c.GetUpperUsage() == c);
            foreach (var con in publicTopMostConnectors)
            {
                yield return new ICDTableProperty() { Port = con };
            }
        }
    }
    private static IEnumerable<ICDTableProperty> SelectConnectorSubs(ICDTableProperty content)
    {
        var port = content.Port;
        // if (port.Definition is PortDefinition_Combined def)
        {
            var subConnectors = port.Definition!.SubPorts;
            foreach (var con in subConnectors)
            {
                yield return new ICDTableProperty() { Port = con };
            }
        }
    }

    [SetsRequiredMembers]
    public PortICD()
    {
        Iterator = new ComponentPropertyIterator<ICDTableProperty>()
        {
            PropertyIterator = SelectPublicTopmostConnectors,
            PropertySubIterator = SelectConnectorSubs,
            DocumentationPerimeter = new DocumentationPerimeter_WithInclusion()
            {
                InclusionCondition = c => c.IsPublic
            },
            WriteBranches = true,
            GroupPNsAtSameLocation = false,
            StackPropertiesSingleChildBranches = false,
        };
        ContentTransform = contents => contents.Where(c =>
            c switch
            {
                IPropertyContent<ICDTableProperty> lp => true,
                _ => c.Component.IsPublic, // Private part are still present as leaf, we remove them
            });
        Columns = [
            IDColumns.ComponentNumberPrettyTree<ICDTableProperty>(
                i =>
                {
                    var prop = i.Property;
                    if(prop.Port.HasStructuralEquivalence)
                       return prop.Port.GetShallowestStructuralEquivalence().GetUpperExposition().Label ?? "";
                    return prop.Port.Label ?? "";
                }),
            ICDColumns.TopMostPortPart(),
            ICDColumns.TopMostPortName(),
            ICDColumns.MostRelevantPortName(),
            ICDColumns.MostRelevantPortName_Regard(),
            ICDColumns.SelfPortName(),
            ];
    }
}
