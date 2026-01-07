using rambap.cplx;
using rambap.cplx.Modules.Costing;

namespace rambap.cplxtests.CoreTests.Costing;

[TestClass]
public class Cost_None : CostCalculationCheck
{
    protected override decimal ExpectedTotalCost => 0;
}

[TestClass]
public class Cost_Value : CostCalculationCheck
{
    Cost handling = 10;
    protected override decimal ExpectedTotalCost => 10;
}

[TestClass]
public class Cost_Sum : CostCalculationCheck
{
    Cost cost0 = 0;
    Cost cost1 = 10;
    Cost cost2 = 200;
    Cost cost3 = 3000;
    protected override decimal ExpectedTotalCost => 3210;
}

[TestClass]
public class Cost_Composition : CostCalculationCheck
{
    Cost_Sum costsum;
    Cost cost4 = 40000;
    protected override decimal ExpectedTotalCost => 43210;
}

[TestClass]
public class Cost_FullOffer : CostCalculationCheck
{
    Offer RS = new()
    {
        Price = 45,
        SKU = "456-789",
        Link = "www.rs.com",
    };
    protected override decimal ExpectedTotalCost => 45;
}

[TestClass]
public class Cost_ImplicitOffer : CostCalculationCheck
{
    Offer RP = 52;
    protected override decimal ExpectedTotalCost => 52;
}

public abstract class CostCalculationCheck: Part
{
    protected abstract decimal ExpectedTotalCost { get;}

    [TestMethod]
    public void TestCostCalculation()
    {
        var component = this.Instantiate();
        if(component.Instance.Cost() is var cost and not null)
        {
            Assert.AreEqual(ExpectedTotalCost, cost.TotalCost);
        }
        else
        {
            Assert.AreEqual(ExpectedTotalCost, 0);
        }
    }
}
