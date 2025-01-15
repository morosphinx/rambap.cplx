using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing.Outputs;
using static rambap.cplx.UnitTests.ExportValidity.ColumnTester;

namespace rambap.cplx.UnitTests.ExportValidity;

[TestClass]
public class TestCostOutputs
{
    private void TestTotalCost_SumCoherence(IIterator<IComponentContent> iterator)
    {
        SetPropertyIterator(iterator, c => c.Instance.Cost()?.NativeCosts ?? []);
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var instance = new Pinstance(part);
        ColumnTester.TestDecimalColumn_SumCoherence(
            instance,
            iterator,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            CostColumns.TotalCost(),
            [
                CostColumns.CostName(),
                CostColumns.UnitCost(),
            ]);
    }

    [TestMethod]
    public void TestTotalCost_SelfTotal()
    {
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var instance = new Pinstance(part);
        TestDecimalColumn_SelfTotal(
            instance,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            CostColumns.TotalCost());
    }

    [TestMethod]
    public void TestTotalCost_1() => TestTotalCost_SumCoherence(ComponentIterator_Flat_NoBranches());
    [TestMethod]
    public void TestTotalCost_2() => TestTotalCost_SumCoherence(ComponentIterator_Recursive_NoBranches());
    [TestMethod]
    public void TestTotalCost_3() => TestTotalCost_SumCoherence(ComponentIterator_Flat_WithBranches());
    [TestMethod]
    public void TestTotalCost_4() => TestTotalCost_SumCoherence(ComponentIterator_Recursive_With_Branches());
    [TestMethod]
    public void TestTotalCost_5() => TestTotalCost_SumCoherence(PartTypeIterator_Flat_NoBranches());
    [TestMethod]
    public void TestTotalCost_6() => TestTotalCost_SumCoherence(PartTypeIterator_Recursive_NoBranches());
    [TestMethod]
    public void TestTotalCost_7() => TestTotalCost_SumCoherence(PartTypeIterator_Flat_WithBranches());
    [TestMethod]
    public void TestTotalCost_8() => TestTotalCost_SumCoherence(PartTypeIterator_Recursive_WithBranches());
    [TestMethod]
    public void TestTotalCost_9() => TestTotalCost_SumCoherence(PartLocationIterator_Flat_NoBranches());
    [TestMethod]
    public void TestTotalCost_10() => TestTotalCost_SumCoherence(PartLocationIterator_Recursive_NoBranches());
    [TestMethod]
    public void TestTotalCost_11() => TestTotalCost_SumCoherence(PartLocationIterator_Flat_WithBranches());
    [TestMethod]
    public void TestTotalCost_12() => TestTotalCost_SumCoherence(PartLocationIterator_Recursive_WithBranches());
}