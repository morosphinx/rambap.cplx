namespace rambap.cplx.Modules.SupplyChain.WorldModel;

public sealed class Supplier
{
    public required Entity Company { get; init; }

    public static implicit operator Supplier(Entity company) => new Supplier() { Company = company };

    public static implicit operator Supplier(string name) => new StringTypedEntityKey() { Name = name };
}
