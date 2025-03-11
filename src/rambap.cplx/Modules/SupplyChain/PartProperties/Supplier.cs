#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public sealed class Supplier
{
    public required Entity Company { get; init; }

    public static implicit operator Supplier(Entity company) => new Supplier() { Company = company };

    public static implicit operator Supplier(string name) => new UnspecifiedCompanyKey() { Name = name };
}
