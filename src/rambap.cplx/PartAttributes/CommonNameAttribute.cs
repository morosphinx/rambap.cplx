namespace rambap.cplx.PartAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CommonNameAttribute : Attribute
{
    public string CommonName { get; init; }

    public CommonNameAttribute(string commonName)
    {
        this.CommonName = commonName;
    }
}
