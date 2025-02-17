using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ICDTableIterator : IIterator<IComponentContent>
{
    public class ICDTableContentProperty
    {
        public required Port Port { get; init; }
    }

    public IEnumerable<IComponentContent> MakeContent(Pinstance instance)
    {
        // Iterate all public connector of a part
        IEnumerable<ICDTableContentProperty> SelectPublicTopmostConnectors(Component component)
        {
            var connectivity = component.Instance.Connectivity();
            if (connectivity != null)
            {
                var publicConnectors = connectivity.Connectors.Where(c => c.IsPublic);
                var publicTopMostConnectors = publicConnectors.Where(c => c.GetTopMostUser() == c);
                foreach (var con in publicTopMostConnectors)
                {
                   yield return new ICDTableContentProperty() { Port = con };
                }
            }
        }

        // Iterate all subconnectors
        IEnumerable<ICDTableContentProperty> SelectConnectorSubs(ICDTableContentProperty content)
        {
            var port = content.Port;
            // if (port.Definition is PortDefinition_Combined def)
            {
                var subConnectors = port.Definition!.SubPorts;
                foreach (var con in subConnectors)
                {
                    yield return new ICDTableContentProperty() { Port = con };
                }
            }
        }

        // Iterate all public components in the hierarchy
        var componentIterator = new ComponentPropertyIterator<ICDTableContentProperty>()
        {
            PropertyIterator = SelectPublicTopmostConnectors,
            PropertySubIterator = SelectConnectorSubs,
            RecursionCondition = (c, l) => c.IsPublic,
            WriteBranches = true,
            GroupPNsAtSameLocation = false,
            StackPropertiesSingleChildBranches = false,
        };

        var contents = componentIterator.MakeContent(instance);

        // Remove undesirable contents
        var filteredContents = contents.Where(c =>
            c switch
            {
                IPropertyContent<ICDTableContentProperty> lp => true,
                _ => c.Component.IsPublic, // Private part are still present as leaf, we remove them
            });
            
        foreach (var c in filteredContents)
        {
            yield return c;
        }
    }

    public IEnumerable<IComponentContent> ExplicitConnectors(IEnumerable<ComponentContent> contents)
    {
        throw new NotImplementedException();
        // return ComponentPropertyIterator<ICDTableContentProperty>.SubIterate(contents,
        //     c => c switch
        //     {
        //         IPropertyContent<ICDTableContentProperty> lp => [], 
        //         LeafComponent lc => [c],
        //         BranchComponent bc => [c],
        //         _ => throw new NotImplementedException(),
        //     });
    }
}
