using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.Core;

public abstract class PropertyGroup
{
}

public class PropertyGroup<T> : PropertyGroup
    where T : Part
{
    public T Part;
}
