using rambap.cplx.Export.Columns;

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
        ColumnTester.TestComponentTreeColumn_Decimal(
            i,
            Tasks.RecurentTaskDuration(),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }

    [TestMethod]
    public void TestColumn_RecurentTaskTotalDuration()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTreeColumn_Decimal(
            i,
            Tasks.TaskTotalDuration(includeNonRecurent:false),
            i => i.Tasks()?.RecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT);
    }

    [TestMethod]
    public void TestColumn_NonRecurentTaskTotalDuration()
    {
        var part = new DecimalPropertyPartExemple<NonRecurrentTask>.Part_A();
        var i = new Pinstance(part);
        ColumnTester.TestPartTreeColumn_Decimal(
            i,
            Tasks.TaskTotalDuration(includeNonRecurent:true),
            i => i.Tasks()?.NonRecurentTasks ?? [],
            DecimalPropertyPartExemple.ExpectedTotal_IntensiveT);
    }
}