using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.PartProperties;
using System.Security.Cryptography.X509Certificates;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ICDTableIterator : IIterator<IComponentContent>
{
    public class ICDTableContentProperty
    {
        public required SignalPort Port { get; init; }
    }

    public IEnumerable<IComponentContent> MakeContent(Pinstance instance)
    {
        // Iterate all public connector of a part
        IEnumerable<object> SelectPublicConnectors(Component component)
        {
            var connectivity = component.Instance.Connectivity();
            if (connectivity != null)
            {
                var publicConnectors = connectivity.Connectors.Where(c => c.IsPublic);
                foreach (var con in publicConnectors)
                {
                   yield return new ICDTableContentProperty() { Port = con };
                }
            }
        }

        // Iterate all public components in the hierarchy
        var componentIterator = new ComponentIterator()
        {
            PropertyIterator = SelectPublicConnectors,
            RecursionCondition = (c, l) => c.IsPublic,
            WriteBranches = true,
            GroupPNsAtSameLocation = false,
            StackPropertiesSingleChildBranches = false,
        };

        var contents = componentIterator.MakeContent(instance);

        // Remove Leaf components, that have no public connector
        var connectorContents = contents.Where(c => c is not LeafComponent);
        // List all public topMostConnectors in the hierarchy
        var topMostConnectors = connectorContents.OfType<IPropertyContent>().Select(c => (c.Property as ICDTableContentProperty)!.Port.GetTopMostUser()).ToList();
        
        // Return true if the SignalPort is a subPort of a public TopMostconnector (combined or exposed as a topmostConnector)
        bool IsSubOfTopMostConnectors(SignalPort port)
        {
            bool isATopMostConnector = topMostConnectors.Contains(port);
            bool isSubOfATopMost = topMostConnectors.Contains(port.GetTopMostUser());
            return !isATopMostConnector && isSubOfATopMost;
        }
        // Remove subPort that will be displayed/included as another TopMostPort
        var topmostConnectorContents = contents.Where(c =>
            c switch
            {
                IPropertyContent { Property : ICDTableContentProperty prop } => ! IsSubOfTopMostConnectors(prop.Port),
                not IPropertyContent => c.Component.IsPublic, // Private part are still present as leaf, we remove them
                _ => false,
            });
            
        // Recursively explicit the content of each SignalPort still in the hierarchy
        var topmostConnectorsR = ComponentIterator.SubIterateProperties<ICDTableContentProperty>(topmostConnectorContents,
            content => content.Port.Definition!.SubPorts.Select(p => new ICDTableContentProperty() { Port = p }));

        foreach (var c in topmostConnectorsR)
        {
            yield return c;
        }
    }

    public IEnumerable<IComponentContent> ExplicitConnectors(IEnumerable<ComponentContent> contents)
    {
        return ComponentIterator.SubIterate(contents,
            c => c switch
            {
                IPropertyContent { Property : ICDTableContentProperty prop} lp => [], 
                LeafComponent lc => [c],
                BranchComponent bc => [c],
                _ => throw new NotImplementedException(),
            });
    }
}
