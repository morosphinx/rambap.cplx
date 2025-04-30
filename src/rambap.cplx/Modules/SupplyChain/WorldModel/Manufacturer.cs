namespace rambap.cplx.Modules.SupplyChain.WorldModel;

public sealed class Manufacturer
{
    public required Entity Company { get; init; }

    public static implicit operator Manufacturer(Entity company) => new Manufacturer() { Company = company };

    public static implicit operator Manufacturer(string name) => new StringTypedEntiryKey() { Name = name };
}

