using rambap.cplx.Export.Columns;

namespace rambap.cplx.UnitTests.ExportValidity;


[TestClass]
public class TestTaskOutputs
{
    // Tested Methods
    [TestMethod]
    public void TestColumn_CostBreakdown_Value()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestComponentTreeColumn_Decimal(
            i,
            Tasks.RecurentTaskDuration(),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotalT);
    }

    [TestMethod]
    public void TestColumn_GroupTotalCost()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTreeColumn_Decimal(
            i,
            Tasks.TaskTotalDuration(),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotalT);
    }
}