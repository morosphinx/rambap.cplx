using rambap.cplx.Core;
using rambap.cplx.Modules.Mass;
using rambap.cplx.Modules.Documentation;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Connectivity;

namespace rambap.cplx;

/// <summary> Extension methods used to manipulate <see cref="Pinstance"/>s</summary>
public static class Extensions
{
    public static InstanceDocumentation? Descriptions(this Pinstance instance)
        => instance.Properties.OfType<InstanceDocumentation>().FirstOrDefault();
    public static InstanceCost? Cost(this Pinstance instance)
        => instance.Properties.OfType<InstanceCost>().FirstOrDefault();
    public static InstanceManufacturerInformation? Manufacturer(this Pinstance instance)
        => instance.Properties.OfType<InstanceManufacturerInformation>().FirstOrDefault();
    public static InstanceMass? Mass(this Pinstance instance)
        => instance.Properties.OfType<InstanceMass>().FirstOrDefault();
    public static InstanceMechanicalAssembly? MechanicalAssembly(this Pinstance instance)
        => instance.Properties.OfType<InstanceMechanicalAssembly>().FirstOrDefault();
    public static InstanceTasks? Tasks(this Pinstance instance)
        => instance.Properties.OfType<InstanceTasks>().FirstOrDefault();
}

