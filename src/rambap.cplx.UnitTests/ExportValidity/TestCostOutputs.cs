using rambap.cplx.Concepts.Costing.Outputs;

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
            CostColumns.CostBreakdown_Value(),
            i => i.Cost()?.NativeCosts ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }

    [TestMethod]
    public void TestColumn_GroupTotalCost()
    {
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTreeColumn_Decimal(
            i,
            CostColumns.GroupTotalCost(),
            i => i.Cost()?.NativeCosts ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }
}