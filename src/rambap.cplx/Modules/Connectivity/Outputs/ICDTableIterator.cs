﻿using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
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
        IEnumerable<object> SelectPublicConnectors(Pinstance instance)
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
            PropertyIterator = SelectPublicConnectors,
            RecursionCondition = (c, l) => c.IsPublic,
            WriteBranches = true
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
            var icdTableProp = (ICDTableContentProperty)c.Property!;
            yield return c;
        }
    }
}
