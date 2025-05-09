using rambap.cplx.PartProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

// Helper methods to manipulate a Pinstance Port
public abstract partial class Port
{
    internal IEnumerable<Port> GetExpositionColumn()
    {
        return
            [
                .. GetExpositionParents(),
                this,
                .. GetExpositionChilds(),
            ];
    }
    private IEnumerable<Port> GetExpositionParents()
        => Usage is PortUsage_ExposedAs use ? use.ExposedAs.GetExpositionParents() : [];
    private IEnumerable<Port> GetExpositionChilds()
        => Definition is PortDefinition_Exposed def ? def.ExposedPort.GetExpositionChilds() : [];

    internal IEnumerable<SignalPortConnection> ExpositionColumnConnection()
        => GetExpositionColumn().SelectMany(c => c.Connections);

    public bool IsExpositionColumnUsageCombined()
        => Usage switch
        {
            PortUsage_CombinedInto => true,
            PortUsage_ExposedAs u => u.User.IsExpositionColumnUsageCombined(),
            null => false,
            _ => throw new NotImplementedException(),
        };


    public bool HasImmediateStructuralEquivalence =>
        Connections.OfType<StructuralConnection>().Any();
    public Port GetImmediateStructuralEquivalence()
        => Connections.OfType<StructuralConnection>().Single().GetOtherSide(this);

    public bool HasStructuralEquivalence =>
        HasImmediateStructuralEquivalence || (Definition is PortDefinition_Exposed { ExposedPort.HasStructuralEquivalence: true });

    public Port GetShallowestStructuralEquivalence()
    {
        if (HasImmediateStructuralEquivalence)
            return GetImmediateStructuralEquivalence();
        else if (Definition is PortDefinition_Exposed dexp)
            return dexp.ExposedPort.GetShallowestStructuralEquivalence();
        else
            throw new InvalidOperationException("No Structural equivalence on this port");
    }



    public PSignal? GetUpperSignal()
        => GetExpositionColumn()
            .Select(p => p.AssignedSignal)
            .Where(s => s != null)
            .FirstOrDefault();
    public Port GetUpperUsage()
    {
        return Usage switch
        {
            PortUsage usage => usage.User.GetUpperUsage(),
            null => this,
        };
    }
    internal Port GetUpperExposition()
    {
        return Usage switch
        {
            PortUsage_ExposedAs usage => usage.User.GetUpperExposition(),
            PortUsage_CombinedInto usage => this,
            null => this,
            _ => throw new NotImplementedException(),
        };
    }
    internal Port GetLowerExposition()
    {
        // = return GetExpositionColumn().First();
        return Definition switch
        {
            PortDefinition_AdHoc def => this,
            PortDefinition_Exposed def => def.ExposedPort.GetLowerExposition(),
            PortDefinition_Combined def => this,
            null => this,
            _ => throw new NotImplementedException(),
        };
    }

    public string FullDefinitionName()
    {
        var localName = $"{Label}";
        if (!HasBeenUseDefined)
            return localName;
        else if (Usage is PortUsage_ExposedAs ue)
            return $"{ue.User.FullDefinitionName()}({localName})";
        else if (Usage is PortUsage_CombinedInto uc)
            return $"{uc.User.FullDefinitionName()}.{localName}";
        else throw new NotImplementedException();
    }
}
