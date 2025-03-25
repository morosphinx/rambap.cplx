using rambap.cplx.Core;
using rambap.cplx.PartProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartInterfaces;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class SignalBuilder
{
    internal Part ContextPart { get; }
    internal Pinstance ContextInstance { get; }

    internal SignalBuilder(Pinstance instance, Part part)
    {
        ContextPart = part;
        ContextInstance = instance;
    }

    public void Assign(Signal signal, WireablePort to)
    {
        AssignBase(signal, to);
    }

    internal static void AssignBase(Signal signal, WireablePort to)
    {
        if (!signal.Assignations.Contains(to))
        {
            to.LocalImplementation.AssignedSignal = signal.Implementation;
            signal.Assignations.Add(to);
        }
    }
}
