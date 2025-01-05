﻿using rambap.cplx.Core;
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
            (CopiedDefinition a, CopiedDefinition b) => AreCompatible(a.CopiedPort.Definition!, b.CopiedPort.Definition!),
            (CopiedDefinition a, AdHocDefinition b) => AreCompatible(a.CopiedPort.Definition!, b),
            (CopiedDefinition a, CombinedDefinition b) => AreCompatible(a.CopiedPort.Definition!, b),
            (AdHocDefinition a, CopiedDefinition b) => AreCompatible(a, b.CopiedPort.Definition!),
            (CombinedDefinition a, CopiedDefinition b) => AreCompatible(a, b.CopiedPort.Definition!),
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

    private ISignalPortConnection? ExclusiveConnection { get; set; }
    private List<ISignalPortConnection> NonExclusiveConnections { get; } = new();

    public void AddConnection(ISignalPortConnection connection)
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

    internal IEnumerable<ISignalPortConnection> Connections
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
            CopiedDefinition d => [d.CopiedPort],
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
    internal class CopiedDefinition : PortDefinition // Exposed connector
    {
        public override IEnumerable<SignalPort> SubPorts => CopiedPort.Definition!.SubPorts;
        public required SignalPort CopiedPort { get; init; }
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
        Definition = new CopiedDefinition() { CopiedPort = source };
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
