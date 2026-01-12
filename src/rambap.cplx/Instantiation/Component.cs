using rambap.cplx.Attributes;
using rambap.cplx.Core;
using System.ComponentModel;
using static rambap.cplx.Core.Support;

namespace rambap.cplx.Instantiation;

/// <summary>
/// Component Instance. The realisation of a Component in the hierarchy, with calculated relations
/// </summary>
public class Component
{

    internal Component(
        Component? parent,
        Pinstance template)
    {
        Parent = parent;
        Instance = template;
    }

    /// <summary>
    /// Definition if this component 
    /// </summary>
    /// Rigth now, Pinstances are created as unique C# class instance, due to Parts also begin created
    /// as unique instances of the Part class, even when reused identicaly in diferent contexts.
    /// This is wastefull, and could one day be optimised to allow components to share Pinstance class instances.
    /// Therefore a Component IS NOT a Pinstance itself, but point to one.
    public Pinstance Instance { get; }

    /// <summary>
    /// Component that contains this component in his SubComponents <br/>
    /// If null, this is the root component.
    /// </summary>
    internal Component? Parent { get; }

    /// <summary>
    /// Component Number : Identifier of this component in its owner
    /// </summary>
    public required string CN { get; init; }

    public string CID(string separator = Core.CID.Separator)
    {
        if (Parent == null)
            return CN;
        else
            return Parent!.CID() + separator + CN;
    }

    /// <summary>
    /// Bubble to the topmost component, creating a stack of components with the root-most on top
    /// </summary>
    private Stack<Component> GetHierarchy()
    {
        Stack<Component> hierarchy = new();
        var currentComponent = this;
        do
        {
            hierarchy.Push(currentComponent);
            currentComponent = currentComponent?.Parent;
        } while (currentComponent != null);
        return hierarchy;
    }

    /// <summary>
    /// Return the CID of the current component, removing all identifier not in the documentation perimeter from the CID
    /// </summary>
    public string CID(DocumentationPerimeter perimeter, string separator = Core.CID.Separator)
    {
        var hierarchy = GetHierarchy();
        var currentComponent = hierarchy.Pop();
        var CID = currentComponent.CN;
        while (hierarchy.Count > 0
            && perimeter.ShouldThisComponentInternalsBeSeen(currentComponent))
        {
            currentComponent = hierarchy.Pop();
            CID += separator + currentComponent.CN;
        }
        return CID;
    }

    /// <summary>
    /// Comment relative to this component - eg : his purpose or usage in its owner
    /// </summary>
    public string Comment { get; init; } = "";

    /// <summary>
    /// True if this component is public or internal from this part : eg, it is visible from outside the containing Part,
    /// and therefore may be condidered to be part or the public interface of the Part.
    /// </summary>
    public required bool IsPublic { get; init; }

    /// <summary>
    /// Immediate sub-Components of this component. All are owned by this component.
    /// </summary>
    public IEnumerable<Component> SubComponents => subComponents;
    private List<Component> subComponents { get; } = new();

    internal void AddComponent(Component component)
    {
        if (component.Parent != this)
            throw new InvalidOperationException("Component's parent is wrong");
        subComponents.Add(component);
    }

    internal void AddConceptPart(Part part)
    {
        // Create Component
        var backupCN = subComponents.Count.ToString();
        var instance = new Pinstance(part);
        var newComponent = new Component(this, instance)
        {
            CN = $"@AUTO:{(string.IsNullOrEmpty(part.CNOverride) ? backupCN : part.CNOverride)}",
            IsPublic = false,
        };
        instance.SetUser(newComponent);
        // TBD / TODO : add to another component list ? 
        subComponents.Add(newComponent);
    }


    /// <summary>
    /// Wrapper for <see cref="Pinstance.PN"/> <br/>
    /// <inheritdoc cref="Pinstance.PN"/> 
    /// </summary>
    public string PN => Instance.PN;
}

