using rambap.cplx.Attributes;
using System.Reflection;

namespace rambap.cplx.Core;

/// <summary>
/// Classify parts depending on their interest <br/>
/// Used during output generation, to determine witch part should be documented,
/// and witch should be considered COTS (and thus have their internals hidden) <br/>
/// </summary>
/// <remarks>
/// Default implementation consider all not in the root part's assembly to be COTS, <br/>
/// as well as any part implementing the <see cref="cplx.Attributes.CplxHideContentsAttribute"/>
/// </remarks>
public record class DocumentationPerimeter
{
    protected bool IsExplicitCOTS(Type partType)
        => partType.GetCustomAttribute(typeof(CplxHideContentsAttribute)) != null;

    protected bool IsInRootPartAssembly(Type partType)
        => partType.Assembly == CurrentRootPartType!.Assembly;

    internal Type? CurrentRootPartType { get; set; } // TODO : do not store here, be immutable

    public virtual bool ShouldThisComponentInternalsBeSeen(Component component)
    {
        var partType = component.Instance.PartType;
        if (IsExplicitCOTS(partType)) return false;
        else return IsInRootPartAssembly(partType);
    }
}

public sealed record class DocumentationPerimeter_WithInclusion : DocumentationPerimeter
{
    public required Func<Component, bool> InclusionCondition { get; init; }

    public override bool ShouldThisComponentInternalsBeSeen(Component component)
    {
        var partType = component.Instance.PartType;
        if (IsExplicitCOTS(partType)) return false;
        else return InclusionCondition(component);
    }
}


public sealed record class DocumentationPerimeter_SinglePart : DocumentationPerimeter
{
    public override bool ShouldThisComponentInternalsBeSeen(Component component)
    {
        // A part cannot contains an instance of itself in cplx.
        // Therefore this guarantee we only recurse on the root part.
        return component.Instance.PartType == CurrentRootPartType;
    }
}
