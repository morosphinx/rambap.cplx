
namespace rambap.cplx.UnitTests.Costing;


internal class PartWithSupplier1 : Part
{
    SupplierOffer Offer1 = new()
    {
        Cost = 45,
        Supplier = "RS",
        Link = "www.rs.com",
    };

    SupplierOffer UnselectedOffer = new()
    {
        Cost = 52,
        Supplier = "RP",
        Link = "www.rp.com",
    };

    Cost handling = 10;

    public static decimal ExpectedCost = 45 + 10;
}

[TestClass]
public class TestSupplyChain1
{
    [TestMethod]
    public void Test1()
    {
        var p = new PartWithSupplier1();
        var i = new Pinstance(p);
        var cost = i.Cost()!;
        Assert.AreEqual(PartWithSupplier1.ExpectedCost, cost.Total);
    }
}
