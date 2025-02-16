using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing;
using rambap.cplx.Modules.Costing.Outputs;
using static rambap.cplx.UnitTests.ExportValidity.ColumnTester;

namespace rambap.cplx.UnitTests.ExportValidity;

[TestClass]
public class TestCostOutputs
{
    private void TestTotalCost_SumCoherence_ComponentIterator(bool recursive, bool writeBranches, bool groupPNsAtSameLocation = false)
        => TestTotalCost_SumCoherence(new ComponentPropertyIterator<InstanceCost.NativeCostInfo>()
        {
            RecursionCondition = (c, l) => recursive,
            WriteBranches = writeBranches,
            PropertyIterator = c => c.Instance.Cost()?.NativeCosts ?? [],
            GroupPNsAtSameLocation = groupPNsAtSameLocation,
        });

    private void TestTotalCost_SumCoherence_PartTypeIterators(bool recursive, bool writeBranches)
    => TestTotalCost_SumCoherence(new PartTypesIterator<InstanceCost.NativeCostInfo>()
    {
        RecursionCondition = (c, l) => recursive,
        WriteBranches = writeBranches,
        PropertyIterator = c => c.Instance.Cost()?.NativeCosts ?? [],
    });

    private void TestTotalCost_SumCoherence(IIterator<IComponentContent> iterator)
    {
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var instance = new Pinstance(part);
        ColumnTester.TestDecimalColumn_SumCoherence<InstanceCost.NativeCostInfo>(
            instance,
            iterator,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            CostColumns.TotalCost(),
            pc => pc.Property.name,
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
    public void TestTotalCost_1() => TestTotalCost_SumCoherence_ComponentIterator(false, false);
    [TestMethod]
    public void TestTotalCost_2() => TestTotalCost_SumCoherence_ComponentIterator(true, false);
    [TestMethod]
    public void TestTotalCost_3() => TestTotalCost_SumCoherence_ComponentIterator(false, true);
    [TestMethod]
    public void TestTotalCost_4() => TestTotalCost_SumCoherence_ComponentIterator(true, true);
    [TestMethod]
    public void TestTotalCost_5() => TestTotalCost_SumCoherence_PartTypeIterators(false, false);
    [TestMethod]
    public void TestTotalCost_6() => TestTotalCost_SumCoherence_PartTypeIterators(true, false);
    [TestMethod]
    public void TestTotalCost_7() => TestTotalCost_SumCoherence_PartTypeIterators(false, true);
    [TestMethod]
    public void TestTotalCost_8() => TestTotalCost_SumCoherence_PartTypeIterators(true, true);
    [TestMethod]
    public void TestTotalCost_9() => TestTotalCost_SumCoherence_ComponentIterator(false, false, true);
    [TestMethod]
    public void TestTotalCost_10() => TestTotalCost_SumCoherence_ComponentIterator(true, false, true);
    [TestMethod]
    public void TestTotalCost_11() => TestTotalCost_SumCoherence_ComponentIterator(false, true, true);
    [TestMethod]
    public void TestTotalCost_12() => TestTotalCost_SumCoherence_ComponentIterator(true, true, true);
}