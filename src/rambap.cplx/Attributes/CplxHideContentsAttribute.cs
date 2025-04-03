namespace rambap.cplx.Attributes;

/// <summary>
/// Apply this atttribute to a part whose contents should NOT be looked into when running <see cref="rambap.cplx.Export.Iterators">
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CplxHideContentsAttribute : Attribute
{
}
