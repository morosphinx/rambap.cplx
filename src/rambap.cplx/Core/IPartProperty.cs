namespace rambap.cplx.Core;

/// <summary>
/// Implemented by part properties that need to know their owner. Autocompleted by <see cref="CplxImplicitInitialisation"/> <br/>
/// Derived classes must have a parameterless constructor
/// </summary>
public abstract class IPartProperty
{
    public string? Name { get; internal set; }
    public bool IsPublic { get; internal set; }

    internal Part? owner = null;
    internal Part? Owner
    {
        get => owner;
        set
        {
            if (owner != null)
                throw new InvalidOperationException(
                    $"Property {Name} ({GetType().Name}) has already been assigned an owner. \n" +
                    "All Part properties must be used in their declaring class. Do not pass a PartProperty to a Part constructor"
                    );
            owner = value;
        }
    }
}

