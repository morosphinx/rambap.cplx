using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.Core;

/// <summary>
/// Group multiple properties or Part to be composed into a <see cref="Part"/> when <see cref="Part.Instantiate()"/>'d
/// </summary>
public abstract class AbstractPropertyGroup
{
    internal abstract void SetPart(Part part);
}

/// <summary>
/// <inheritdoc cref="AbstractPropertyGroup"/><br/>
/// Can be added to any Part <see cref="T"/> or <see cref="PropertyGroup{T}">
/// </summary>
/// <typeparam name="T">Type of part this is to be composed into.<br/>
/// cplx will throw if you add this PropertyGroup to a part not assignable to this type <see cref="T"/>.</typeparam>
public class PropertyGroup<T> : AbstractPropertyGroup
    where T : Part
{
    /// <summary>
    /// The part containing this property group. <br/>
    /// This is set by cplx, <b>after</b> the <see cref="PropertyGroup{T}"/> construction. <br/>
    /// Use this to define unbacked properties that reference other components
    /// </summary>
    private T? part;
    public T Part
    {
        get => part ?? throw new InvalidOperationException("The cplx part instantiation has not yet been run. "
            + $"You cannot access the Part object during {nameof(PropertyGroup<T>)} construction.");
        private set => part = value;
    }

    internal override void SetPart(Part part)
    {
        try
        {
            Part = (T)part;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidOperationException($"Incorrect target for property group type."
                + $" This part is a {part.GetType()} while this property group is only valid on a {nameof(T)} or a derived part type.");
        }
    }
}

/// <summary>
/// <inheritdoc cref="AbstractPropertyGroup"/><br/>
/// Can be added to any Part or PropertyGroup
/// </summary>
public class PropertyGroup : PropertyGroup<Part> { }
