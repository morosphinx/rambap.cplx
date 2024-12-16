#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public abstract class Company
{
    public abstract string Name { get; init; }
}
internal sealed class UnspecifiedCompanyKey : Company
{
    public override required string Name {get ; init ;}
}

public class Manufacturer
{
    public required Company Company { get; init; }

    public static implicit operator Manufacturer(Company company) => new Manufacturer() { Company = company };

    public static implicit operator Manufacturer(string name) => new UnspecifiedCompanyKey() { Name = name };
}

