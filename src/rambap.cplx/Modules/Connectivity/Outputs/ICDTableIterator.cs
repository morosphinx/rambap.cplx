using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;
using rambap.cplx.PartProperties;
using System.Security.Cryptography.X509Certificates;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public class ICDTableIterator : IIterator<ComponentContent>
{
    public class ICDTableContentProperty
    {
        public required SignalPort Port { get; init; }
    }

    public IEnumerable<ComponentContent> MakeContent(Pinstance instance)
    {
        IEnumerable<object> SelectConnector(Pinstance instance)
        {

            var connectivity = instance.Connectivity();
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
            PropertyIterator = SelectConnector,
            RecursionCondition = (c, l) => c.IsPublic,
            WriteBranches = false
        };

        var contents = componentIterator.MakeContent(instance);
        var connectorContents = contents.Where(c => c is not LeafComponent);

        var topMostConnectors = connectorContents.OfType<LeafProperty>().Select(c => (c.Property as ICDTableContentProperty)!.Port.TopMostUser()).ToList();
        bool IsSubOfTopMostConnectors(SignalPort port)
        {
            bool isATopMostConnector = topMostConnectors.Contains(port);
            bool isSubOfATopMost = topMostConnectors.Contains(port.TopMostUser());
            return !isATopMostConnector && isSubOfATopMost;
        }
        var topmostConnectorContents = contents.OfType<LeafProperty>().Where(c =>
            ! IsSubOfTopMostConnectors((c.Property as ICDTableContentProperty)!.Port));

        foreach (var c in topmostConnectorContents)
        {
            yield return c;
        }
    }
}
