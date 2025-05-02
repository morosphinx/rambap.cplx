using rambap.cplx;
using rambap.cplx.Modules.Costing;

namespace rambap.cplxtests.CoreTests.Costing;

internal class PartWithSupplier1 : Part
{
    Offer RS = new()
    {
        Price = 45,
        SKU = "456-789",
        Link = "www.rs.com",
    };
    
    Offer RP = 52;

    Cost handling = 10; // Cost are Additional to the selected offer

    public static decimal ExpectedCost = 55 ; 
}

[TestClass]
public class TestSupplyChain1
{
    [TestMethod]
    public void Test1()
    {
        var p = new PartWithSupplier1();
        var c = p.Instantiate();
        var cost = c.Instance.Cost()!;
        Assert.AreEqual(PartWithSupplier1.ExpectedCost, cost.Total);
    }
}
