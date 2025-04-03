using rambap.cplx.Core;

namespace rambap.cplx.Modules.Connectivity.PinstanceModel;

/// <summary>
/// Instance model implementation of a <see cref="PartProperties.Signal"/>
/// </summary>
public class PSignal
{
    public string Label { get; internal set; }
    public Pinstance Owner { get; }
    public bool IsPublic { get; internal set; }
    internal PSignal(string label, Pinstance owner, bool isPublic)
    {
        Label = label;
        Owner = owner;
        IsPublic = isPublic;
    }
}
