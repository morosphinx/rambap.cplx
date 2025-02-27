namespace rambap.cplx.Core;

/// <summary>
/// Implemented by part properties that need to know their owner. Autocompleted by <see cref="CplxImplicitInitialisation"/> <br/>
/// Derived classes must have a parameterless constructor
/// </summary>
public abstract class IPartProperty
{
    public string? Name { get; internal set; }
    public bool IsPublic { get; internal set; }

    internal Part? Owner { get; set; }
}

