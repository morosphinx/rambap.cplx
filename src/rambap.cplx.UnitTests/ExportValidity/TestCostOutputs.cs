using rambap.cplx.Export.Iterators;
using rambap.cplx.Modules.Costing.Outputs;
using static rambap.cplx.UnitTests.ExportValidity.ColumnTester;

namespace rambap.cplx.UnitTests.ExportValidity;

[TestClass]
public class TestCostOutputs
{
    private void SetPropertyIterator(IIterator<ComponentContent> iterator,
        Func<Pinstance, IEnumerable<object>>? propertyIterator)
    {
        switch(iterator)
        {
            case ComponentIterator ci:
                ci.PropertyIterator = propertyIterator;
                break;
            case PartTypesIterator pi:
                pi.PropertyIterator = propertyIterator;
                break;
            default: throw new NotImplementedException();
        };
    }

    private void TestGroupTotalCost_SumCoherence(IIterator<ComponentContent> iterator)
    {
        SetPropertyIterator(iterator, i => i.Cost()?.NativeCosts ?? []);
        var part = new DecimalPropertyPartExemple<Cost>.Part_A();
        var instance = new Pinstance(part);
        ColumnTester.TestDecimalColumn_SumCoherence(
            instance,
            iterator,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            CostColumns.GroupTotalCost(),
            [
                CostColumns.Group_CostName(),
                CostColumns.Group_UnitCost(),
            ]);
    }

    [TestMethod]
    public void TestGroupTotalCost_1() => TestGroupTotalCost_SumCoherence(ComponentIterator_Flat_NoBranches());
    [TestMethod]
    public void TestGroupTotalCost_2() => TestGroupTotalCost_SumCoherence(ComponentIterator_Recursive_NoBranches());
    [TestMethod]
    public void TestGroupTotalCost_3() => TestGroupTotalCost_SumCoherence(ComponentIterator_Flat_WithBranches());
    [TestMethod]
    public void TestGroupTotalCost_4() => TestGroupTotalCost_SumCoherence(ComponentIterator_Recursive_With_Branches());
    [TestMethod]
    public void TestGroupTotalCost_5() => TestGroupTotalCost_SumCoherence(PartTypeIterator_Flat_NoBranches());
    [TestMethod]
    public void TestGroupTotalCost_6() => TestGroupTotalCost_SumCoherence(PartTypeIterator_Recursive_NoBranches());
    [TestMethod]
    public void TestGroupTotalCost_7() => TestGroupTotalCost_SumCoherence(PartTypeIterator_Flat_WithBranches());
    [TestMethod]
    public void TestGroupTotalCost_8() => TestGroupTotalCost_SumCoherence(PartTypeIterator_Recursive_WithBranches());
    [TestMethod]
    public void TestGroupTotalCost_9() => TestGroupTotalCost_SumCoherence(PartLocationIterator_Flat());
    [TestMethod]
    public void TestGroupTotalCost_10() => TestGroupTotalCost_SumCoherence(PartLocationIterator_Recursive());
}