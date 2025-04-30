namespace rambap.cplx.Modules.SupplyChain.WorldModel;

public abstract class Entity
{
    public abstract string Name { get; init; }
}

/// <summary>
/// Helper class that serves as a placeholder during part construction to specify an Entity by string
/// </summary>
internal sealed class StringTypedEntiryKey : Entity
{
    public override required string Name { get; init; }
}