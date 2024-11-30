using rambap.cplx.Export.Columns;

namespace rambap.cplx.UnitTests.ExportValidity;

[TestClass]
public class TestCostOutputs
{
    [TestMethod]
    public void TestColumn_CostBreakdown_Value()
    {
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestComponentTreeColumn_Decimal(
            i,
            Costs.CostBreakdown_Value(),
            i => i.Cost()?.NativeCosts ?? [],
            DecimalPropertyPartExemple.ExpectedTotalT);
    }

    [TestMethod]
    public void TestColumn_GroupTotalCost()
    {
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTreeColumn_Decimal(
            i,
            Costs.GroupTotalCost(),
            i => i.Cost()?.NativeCosts ?? [],
            DecimalPropertyPartExemple.ExpectedTotalT);
    }
}