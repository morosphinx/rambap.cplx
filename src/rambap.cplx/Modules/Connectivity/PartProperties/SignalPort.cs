using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.Model;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public abstract class SignalPort : IPartProperty
{
    /// <summary>
    /// Identifier of this port, visible from outside this interface
    /// </summary>
    public string Label => Name!;

    private Signal? signal;
    public Signal? Signal
    {
        get => signal;
        internal set
        {
            if (signal != null)
                throw new InvalidOperationException("A signal is already assigned to this connector");
            signal = value;
        }
    }

    public static bool AreCompatible(SignalPort A, SignalPort B)
     => A.GetType() == B.GetType() &&
        AreCompatible(A.Definition!, B.Definition!);
    internal static bool AreCompatible(PortDefinition A, PortDefinition B)
    {
        return (A, B) switch
        {
            // Copied connectors : Test one level deeper
            (ExposedDefinition a, ExposedDefinition b) => AreCompatible(a.ExposedPort.Definition!, b.ExposedPort.Definition!),
            (ExposedDefinition a, AdHocDefinition b) => AreCompatible(a.ExposedPort.Definition!, b),
            (ExposedDefinition a, CombinedDefinition b) => AreCompatible(a.ExposedPort.Definition!, b),
            (AdHocDefinition a, ExposedDefinition b) => AreCompatible(a, b.ExposedPort.Definition!),
            (CombinedDefinition a, ExposedDefinition b) => AreCompatible(a, b.ExposedPort.Definition!),
            // Add Hoc : Always compatible with itself
            (AdHocDefinition a, AdHocDefinition b) => true,
            // Add Hoc and combined are never compatible : Even whith a single pin, a combined connector imply an additional containing level
            (AdHocDefinition a, CombinedDefinition b) => false,
            (CombinedDefinition a, AdHocDefinition b) => false,
            // Two Combined : Valid when same size, compatible subconnectors in order
            (CombinedDefinition a, CombinedDefinition b) =>
                a.CombinedPorts.Count == b.CombinedPorts.Count &&
                Enumerable.Range(0, a.CombinedPorts.Count)
                          .All(i => AreCompatible(a.CombinedPorts[i].Definition!, b.CombinedPorts[i].Definition!)),
            // 3 x 3 = 9 total cases, all covered
            _ => throw new NotImplementedException(),
        };
    }

    private SignalPortConnection? ExclusiveConnection { get; set; }
    private List<SignalPortConnection> NonExclusiveConnections { get; } = new();

    public void AddConnection(SignalPortConnection connection)
    {
        if (connection.IsExclusive)
        {
            if (CanAddExclusiveConnection)
                ExclusiveConnection = connection;
            else 
                throw new InvalidOperationException("This connectable port is already connected");
        } else
        {
            NonExclusiveConnections.Add(connection);
        }
    }

    internal IEnumerable<SignalPortConnection> Connections
        => ExclusiveConnection != null
            ? [ExclusiveConnection, .. NonExclusiveConnections]
            : NonExclusiveConnections;

    internal bool CanAddExclusiveConnection =>
        !IsThisExclusivelyConnected &&
        !IsAnyChildsExclusivelyConnected() &&
        !IsAnyParentExclusivelyConnected();

    internal bool IsThisExclusivelyConnected => ExclusiveConnection != null;
    internal bool IsAnyChildsExclusivelyConnected()
    {
        var subdef = Definition switch
        {
            AdHocDefinition d => [],
            ExposedDefinition d => [d.ExposedPort],
            CombinedDefinition d => d.CombinedPorts,
            null => [],
            _ => throw new NotImplementedException(),
        };
        return subdef.Any(d => d.IsThisExclusivelyConnected || d.IsAnyChildsExclusivelyConnected());
    }
    internal bool IsAnyParentExclusivelyConnected()
    {
        var parent = Usage switch
        {
            UsageExposedAs u => u.ExposedAs,
            UsageCombinedInto u => u.CombinedInto,
            null => null,
            _ => throw new NotImplementedException(),
        };
        if (parent != null)
            return parent.IsThisExclusivelyConnected || parent.IsAnyParentExclusivelyConnected();
        else return false;
    }



    // what define this connector

    internal abstract class PortDefinition
    {
        public abstract IEnumerable<SignalPort> SubPorts { get; }
    }
    internal class AdHocDefinition : PortDefinition // Single pin / signal
    {
        public override IEnumerable<SignalPort> SubPorts => [];
    }
    internal class ExposedDefinition : PortDefinition // Exposed connector
    {
        public override IEnumerable<SignalPort> SubPorts => ExposedPort.Definition?.SubPorts ?? [];
        public required SignalPort ExposedPort { get; init; }
    }
    internal class CombinedDefinition : PortDefinition // Exposed / grouped connector
    {
        public override IEnumerable<SignalPort> SubPorts => CombinedPorts ;
        public required List<SignalPort> CombinedPorts { get; init; } // Connector in order, their names is used as the label
    }
    internal PortDefinition? Definition { get; private set; }
    internal bool HasbeenDefined => Definition != null;


    // Wether this used as part of another definition

    internal abstract class PortDefinitionUsage
    {
        public abstract SignalPort User { get; }
    }
    internal class UsageExposedAs : PortDefinitionUsage
    {
        public required SignalPort ExposedAs { get; init; }
        public override SignalPort User => ExposedAs;
    }
    internal class UsageCombinedInto : PortDefinitionUsage
    {
        public required SignalPort CombinedInto { get; init; }
        public override SignalPort User => CombinedInto;
    }
    internal PortDefinitionUsage? Usage { get; private set; }
    internal bool HasBeenUseDefined => Usage != null;

    internal IEnumerable<SignalPort> GetExpositionColumn()
    {
        return
            [
                .. GetExpositionParents(),
                this,
                .. GetExpositionChilds(),
            ];
    }
    private IEnumerable<SignalPort> GetExpositionParents()
        => Usage is UsageExposedAs use ? use.ExposedAs.GetExpositionParents() : [];
    private IEnumerable<SignalPort> GetExpositionChilds()
        => Definition is ExposedDefinition def ? def.ExposedPort.GetExpositionChilds() : [];


    public bool HasImmediateStructuralEquivalence =>
        Connections.OfType<StructuralConnection>().Any();
    public SignalPort GetImmediateStructuralEquivalence()
        => Connections.OfType<StructuralConnection>().Single().GetOtherSide(this);

    public bool HasStructuralEquivalence =>
        HasImmediateStructuralEquivalence || (Definition is ExposedDefinition { ExposedPort.HasStructuralEquivalence: true }) ;

    public SignalPort GetShallowestStructuralEquivalence()
    {
        if (HasImmediateStructuralEquivalence)
            return GetImmediateStructuralEquivalence();
        else if (Definition is ExposedDefinition dexp)
            return dexp.ExposedPort.GetShallowestStructuralEquivalence();
        else
            throw new InvalidOperationException("No Structural equivalence on this port");
    }

    internal SignalPort GetTopMostUser()
    {
        return Usage switch
        {
            PortDefinitionUsage usage => usage.User.GetTopMostUser(),
            null => this,
        };
    }
    internal SignalPort GetTopMostExposition()
    {
        return Usage switch
        {
            UsageExposedAs usage => usage.User.GetTopMostExposition(),
            UsageCombinedInto usage => this,
            null => this,
            _ => throw new NotImplementedException(),
        };
    }
    internal SignalPort GetDeepestExposition()
    {
        // = return GetExpositionColumn().First();
        return Definition switch
        {
            AdHocDefinition def => this,
            ExposedDefinition def => def.ExposedPort.GetDeepestExposition(),
            CombinedDefinition def => this,
            null => this,
            _ => throw new NotImplementedException(),
        };
    }

    public string FullDefinitionName()
    {
        var localName = $"{Name}";
        if (!HasBeenUseDefined)
            return localName;
        else if (Usage is UsageExposedAs ue)
            return $"{ue.User.FullDefinitionName()}({localName})";
        else if (Usage is UsageCombinedInto uc)
            return $"{uc.User.FullDefinitionName()}.{localName}";
        else throw new NotImplementedException();
    }

    internal void DefineAsHadHoc()
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        Definition = new AdHocDefinition();
    }

    internal void DefineAsAnExpositionOf(SignalPort source)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        Definition = new ExposedDefinition() { ExposedPort = source };
        source.Usage = new UsageExposedAs() { ExposedAs = this };
    }

    internal void DefineAsAnExpositionOf(IEnumerable<ConnectablePort> sources)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        foreach (var source in sources)
        {
            if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        }
        Definition = new CombinedDefinition() { CombinedPorts = [.. sources] };
        foreach (var source in sources)
        {
            source.Usage = new UsageCombinedInto() { CombinedInto = this };
        }
    }
}
