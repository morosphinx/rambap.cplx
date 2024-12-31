﻿using rambap.cplx.Core;

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
    internal static bool AreCompatible(ConnectorDefinition A, ConnectorDefinition B)
    {
        return (A, B) switch
        {
            // Copied connectors : Test one level deeper
            (CopiedDefinition a, CopiedDefinition b) => AreCompatible(a.CopiedConnector.Definition!, b.CopiedConnector.Definition!),
            (CopiedDefinition a, AdHocDefinition b) => AreCompatible(a.CopiedConnector.Definition!, b),
            (CopiedDefinition a, CombinedDefinition b) => AreCompatible(a.CopiedConnector.Definition!, b),
            (AdHocDefinition a, CopiedDefinition b) => AreCompatible(a, b.CopiedConnector.Definition!),
            (CombinedDefinition a, CopiedDefinition b) => AreCompatible(a, b.CopiedConnector.Definition!),
            // Add Hoc : Always compatible with itself
            (AdHocDefinition a, AdHocDefinition b) => true,
            // Add Hoc and combined are never compatible : Even whith a single pin, a combined connector imply an additional containing level
            (AdHocDefinition a, CombinedDefinition b) => false,
            (CombinedDefinition a, AdHocDefinition b) => false,
            // Two Combined : Valid when same size, compatible subconnectors in order
            (CombinedDefinition a, CombinedDefinition b) =>
                a.CombinedConnectors.Count == b.CombinedConnectors.Count &&
                Enumerable.Range(0, a.CombinedConnectors.Count)
                          .All(i => AreCompatible(a.CombinedConnectors[i].Definition!, b.CombinedConnectors[i].Definition!)),
            // 3 x 3 = 9 total cases, all covered
            _ => throw new NotImplementedException(),
        };
    }

    internal abstract class ConnectorDefinition{}
    internal class AdHocDefinition : ConnectorDefinition{}// Single pin / signal
    internal class CopiedDefinition : ConnectorDefinition // Exposed connector
    {
        public required SignalPort CopiedConnector { get; init; }
    }
    internal class CombinedDefinition : ConnectorDefinition // Exposed / grouped connector
    {
        public required List<SignalPort> CombinedConnectors { get; init; } // Connector in order, their names is used as the label
    }
    internal ConnectorDefinition? Definition { get; private set; }
    internal bool HasbeenDefined => Definition != null;



    internal abstract class ConnectorDefinitionUsage
    {
        public abstract SignalPort User { get; }
    }
    internal class UsageExposedAs : ConnectorDefinitionUsage
    {
        public required SignalPort ExposedAs { get; init; }
        public override SignalPort User => ExposedAs;
    }
    internal class UsageCombinedInto : ConnectorDefinitionUsage
    {
        public required SignalPort CombinedInto { get; init; }
        public override SignalPort User => CombinedInto;
    }

    internal ConnectorDefinitionUsage? Usage { get; private set; }
    internal bool HasBeenUseDefined => Usage != null;

    internal SignalPort TopMostUser()
    {
        if (HasBeenUseDefined) return Usage!.User.TopMostUser();
        else return this;
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
        Definition = new CopiedDefinition() { CopiedConnector = source };
        source.Usage = new UsageExposedAs() { ExposedAs = this };
    }

    internal void DefineAsAnExpositionOf(IEnumerable<ConnectablePort> sources)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        foreach (var source in sources)
        {
            if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        }
        Definition = new CombinedDefinition() { CombinedConnectors = [.. sources] };
        foreach (var source in sources)
        {
            source.Usage = new UsageCombinedInto() { CombinedInto = this };
        }
    }
}