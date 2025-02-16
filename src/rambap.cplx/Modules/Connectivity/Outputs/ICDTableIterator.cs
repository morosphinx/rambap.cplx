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
        IEnumerable<ICDTableContentProperty> SelectPublicConnectors(Component component)
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
        var componentIterator = new ComponentPropertyIterator<ICDTableContentProperty>()
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
        var topMostConnectors = connectorContents
            .OfType<IPropertyContent<ICDTableContentProperty>>()
            .Select(c => c.Property.Port.GetTopMostUser())
            .ToList();
        
        // Return true if the SignalPort is a subPort of a public TopMostconnector (combined or exposed as a topmostConnector)
        bool IsSubOfTopMostConnectors(Port port)
        {
            bool isATopMostConnector = topMostConnectors.Contains(port);
            bool isSubOfATopMost = topMostConnectors.Contains(port.GetTopMostUser());
            return !isATopMostConnector && isSubOfATopMost;
        }
        // Remove subPort that will be displayed/included as another TopMostPort
        var topmostConnectorContents = contents.Where(c =>
            c switch
            {
                IPropertyContent<ICDTableContentProperty> lp => ! IsSubOfTopMostConnectors(lp.Property.Port),
                _ => c.Component.IsPublic, // Private part are still present as leaf, we remove them
            });
            

        throw new NotImplementedException();
        // Recursively explicit the content of each SignalPort still in the hierarchy
        // var topmostConnectorsR = ComponentPropertyIterator<ICDTableContentProperty>.SubIterateProperties(topmostConnectorContents,
        //     content => content.Port.Definition!.SubPorts.Select(p => new ICDTableContentProperty() { Port = p }));
        // 
        // foreach (var c in topmostConnectorsR)
        // {
        //     yield return c;
        // }
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
