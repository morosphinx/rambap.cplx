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

        var componentIterator = new ComponentIterator()
        {
            PropertyIterator = SelectPublicConnectors,
            RecursionCondition = (c, l) => c.IsPublic,
            WriteBranches = true,
            GroupPNsAtSameLocation = false,
            StackPropertiesSingleChildBranches = false,
        };

        var contents = componentIterator.MakeContent(instance);
        var connectorContents = contents.Where(c => c is not LeafComponent);

        var topMostConnectors = connectorContents.OfType<IPropertyContent>().Select(c => (c.Property as ICDTableContentProperty)!.Port.TopMostUser()).ToList();
        bool IsSubOfTopMostConnectors(SignalPort port)
        {
            bool isATopMostConnector = topMostConnectors.Contains(port);
            bool isSubOfATopMost = topMostConnectors.Contains(port.TopMostUser());
            return !isATopMostConnector && isSubOfATopMost;
        }
        var topmostConnectorContents = contents.Where(c =>
            c switch
            {
                IPropertyContent { Property : ICDTableContentProperty prop } => ! IsSubOfTopMostConnectors(prop.Port),
                not IPropertyContent => c.Component.IsPublic, // Private part are still present as leaf, we remove them
                _ => false,
            });
            

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
