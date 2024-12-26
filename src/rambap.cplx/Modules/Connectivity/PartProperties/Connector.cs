using rambap.cplx.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Define a connectable element, generaly electrical, on a Part. <br/>
/// May represent a single pin, a connector part with multiple pins, or a group of connector parts. <br/>
/// To be then used by Parts implementing <see cref="PartInterfaces.IPartConnectable"/> <br/>
/// Define <see cref="Connector"/> as public when they can be seen and used from outside the Part.
/// </summary>
/// TODO : Find a better name. May cause confusion. "Connecter" is a common Part object name.
/// "Interface" is not acceptable, because "interface MyStuff;" is a valid declaration in a C# class
public class Connector : IPartProperty
{
    /// <summary>
    /// Identifier of this connector, visible from outside this interface
    /// </summary>
    public string Label => Name!;


    public static bool AreCompatible(Connector A, Connector B)
        => AreCompatible(A.Definition!, B.Definition!);
    internal static bool AreCompatible(ConnectorDefinition A, ConnectorDefinition B)
    {
        return (A, B) switch
        {
            // Copied connectors : Test one level deeper
            (CopiedDefinition a, CopiedDefinition b)   => AreCompatible(a.CopiedConnector.Definition!, b.CopiedConnector.Definition!),
            (CopiedDefinition a, AdHocDefinition b)    => AreCompatible(a.CopiedConnector.Definition!, b),
            (CopiedDefinition a, CombinedDefinition b) => AreCompatible(a.CopiedConnector.Definition!, b),
            (AdHocDefinition a, CopiedDefinition b)    => AreCompatible(a, b.CopiedConnector.Definition!),
            (CombinedDefinition a, CopiedDefinition b) => AreCompatible(a, b.CopiedConnector.Definition!),
            // Add Hoc : Always compatible with itself
            (AdHocDefinition a, AdHocDefinition b) => true,
            // Add Hoc and combined are never compatible : Even whith a single pin, a combined connector imply an additional containing level
            (AdHocDefinition a, CombinedDefinition b) => false,
            (CombinedDefinition a, AdHocDefinition b) => false,
            // Two Combined : Valid when same size, compatible subconnectors in order
            (CombinedDefinition a, CombinedDefinition b) =>
                a.CombinedConnectors.Count() == b.CombinedConnectors.Count() &&
                Enumerable.Range(0,a.CombinedConnectors.Count())
                          .All(i => AreCompatible(a.CombinedConnectors[i].Definition!, b.CombinedConnectors[i].Definition!)),
            // 3 x 3 = 9 total cases, all covered
            _ => throw new NotImplementedException(),
        };
    }

    internal abstract class ConnectorDefinition
    {

    }

    internal class AdHocDefinition : ConnectorDefinition
    {
        // Single pin / signal
    }

    internal class CopiedDefinition : ConnectorDefinition
    {
        // Exposed connector
        public required Connector CopiedConnector { get; init; }
    }
    internal class CombinedDefinition : ConnectorDefinition
    {
        // Exposed / grouped connector
        // Connector in order, their names is used as the label
        public required List<Connector> CombinedConnectors { get; init; }
    }

    internal ConnectorDefinition? Definition { get; private set; }
    internal bool HasbeenDefined => Definition != null;

    internal abstract class ConnectorDefinitionUsage
    {
        public abstract Connector User { get; }
    }

    internal class UsageExposedAs : ConnectorDefinitionUsage
    {
        public required Connector ExposedAs { get; init; }
        public override Connector User => ExposedAs;
    }

    internal class UsageCombinedInto : ConnectorDefinitionUsage
    {
        public required Connector CombinedInto { get; init; }
        public override Connector User => CombinedInto;
    }

    internal ConnectorDefinitionUsage? Usage { get; private set; }
    internal bool HasBeenUseDefined => Usage != null;

    internal Connector TopMostUser()
    {
        if (HasBeenUseDefined) return Usage!.User.TopMostUser();
        else return this;
    }

    internal void DefineAsHadHoc()
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        Definition = new AdHocDefinition() ;
    }

    internal void DefineAsAnExpositionOf(Connector source)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        Definition = new CopiedDefinition() { CopiedConnector = source };
    }

    internal void DefineAsAnExpositionOf(IEnumerable<Connector> sources)
    {
        if (HasbeenDefined) throw new InvalidOperationException($"Connector has already been defined");
        foreach(var source in sources)
        {
            if (source.HasBeenUseDefined) throw new InvalidOperationException($"Connector {source} has already been used in another definition ({source.Usage!.User})");
        }
        Definition = new CombinedDefinition() { CombinedConnectors = [.. sources] };
    }




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
}

