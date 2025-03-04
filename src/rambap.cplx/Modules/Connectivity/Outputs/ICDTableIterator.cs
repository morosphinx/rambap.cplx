using rambap.cplx.Core;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Connectivity.PinstanceModel;
using rambap.cplx.PartProperties;

namespace rambap.cplx.Modules.Connectivity.Outputs;

public static class ICDTableIterator
{
    public class ICDTableContentProperty
    {
        public required Port Port { get; init; }
    }
    public static IEnumerable<ICDTableContentProperty> SelectPublicTopmostConnectors(Component component)
    {
        var connectivity = component.Instance.Connectivity();
        if (connectivity != null)
        {
            var publicConnectors = connectivity.Connectors.Where(c => c.IsPublic);
            var publicTopMostConnectors = publicConnectors.Where(c => c.GetUpperUsage() == c);
            foreach (var con in publicTopMostConnectors)
            {
                yield return new ICDTableContentProperty() { Port = con };
            }
        }
    }
    public static IEnumerable<ICDTableContentProperty> SelectConnectorSubs(ICDTableContentProperty content)
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

    public static IEnumerable<ICplxContent> ExplicitConnectors(IEnumerable<CplxContent> contents)
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
