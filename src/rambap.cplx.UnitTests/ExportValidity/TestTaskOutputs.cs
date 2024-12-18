using rambap.cplx.Modules.Costing.Outputs;

namespace rambap.cplx.UnitTests.ExportValidity;


[TestClass]
public class TestTaskOutputs
{
    // Tested Methods
    [TestMethod]
    public void TestColumn_ComponentRecurentTaskDuration()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestComponentIteratorColumn_Decimal(
            i,
            TaskColumns.RecurentTaskDuration(),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }

    [TestMethod]
    public void TestColumn_RecurentTaskTotalDuration()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTypeIteratorColumn_Decimal(
            i,
            TaskColumns.TaskTotalDuration(includeNonRecurent:false),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }

    [TestMethod]
    public void TestColumn_NonRecurentTaskTotalDuration()
    {
        var part = new DecimalPropertyPartExemple<NonRecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTypeIteratorColumn_Decimal(
            i,
            TaskColumns.TaskTotalDuration(includeNonRecurent:true),
            i => i.Tasks()?.NonRecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_IntensiveT);
    }
}