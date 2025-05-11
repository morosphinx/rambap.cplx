using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.Core;

public abstract class PropertyGroup
{
    internal abstract void SetPart(Part part);
}

public class PropertyGroup<T> : PropertyGroup
    where T : Part
{
    public T Part { get; private set; }

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
