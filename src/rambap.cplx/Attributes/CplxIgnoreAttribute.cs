namespace rambap.cplx.Attributes;

/// <summary>
/// Apply this atttribute to a field or property that should NOT processed during calculations <br/>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class CplxIgnoreAttribute : Attribute
{
}

