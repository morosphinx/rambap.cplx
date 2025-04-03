#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public abstract class Entity
{
    public abstract string Name { get; init; }
}

/// <summary>
/// Helper class that serves as a placeholder during part construction to specify an Entity by string
/// </summary>
internal sealed class UnspecifiedEntityKey : Entity
{
    public override required string Name { get; init; }
}
