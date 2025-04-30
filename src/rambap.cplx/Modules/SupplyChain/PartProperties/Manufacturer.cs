using rambap.cplx.Modules.SupplyChain.WorldModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace rambap.cplx.PartProperties;
#pragma warning restore IDE0130 // Namespace does not match folder structure
public sealed class Manufacturer
{
    public required Entity Company { get; init; }

    public static implicit operator Manufacturer(Entity company) => new Manufacturer() { Company = company };

    public static implicit operator Manufacturer(string name) => new StringTypedEntityKey() { Name = name };
}

