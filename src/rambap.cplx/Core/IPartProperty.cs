namespace rambap.cplx.Core;

/// <summary> Implemented by part properties that need to know their owner. Autocompleted  by <see cref="CplxImplicitInitialisation"/></summary>
public abstract class IPartProperty
{
    public string? Name { get; internal set; }
    public Part? Owner { get; internal set; }
}

