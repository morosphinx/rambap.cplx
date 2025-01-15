using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing.Outputs;
using static rambap.cplx.UnitTests.ExportValidity.ColumnTester;

namespace rambap.cplx.UnitTests.ExportValidity;


[TestClass]
public class TestTaskOutputs
{
    private void TestRecurentTask_SumCoherence(IIterator<IComponentContent> iterator)
    {
        SetPropertyIterator(iterator, c => c.Instance.Tasks()?.RecurentTasks ?? []);
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var instance = new Pinstance(part);
        ColumnTester.TestDecimalColumn_SumCoherence(
            instance,
            iterator,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            TaskColumns.TaskTotalDuration(false),
            [
                TaskColumns.TaskName(),
                TaskColumns.TaskCategory(),
            ]);
    }

    [TestMethod]
    public void TestRecurentTask_SelfTotal()
    {
        var part = new DecimalPropertyPartExemple<RecurrentTask>.Part_A();
        var instance = new Pinstance(part);
        TestDecimalColumn_SelfTotal(
            instance,
            DecimalPropertyPartExemple.ExpectedTotal_ExtensiveT,
            TaskColumns.TaskTotalDuration(false));
    }

    [TestMethod]
    public void TestRecurentTask_1() => TestRecurentTask_SumCoherence(ComponentIterator_Flat_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_2() => TestRecurentTask_SumCoherence(ComponentIterator_Recursive_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_3() => TestRecurentTask_SumCoherence(ComponentIterator_Flat_WithBranches());
    [TestMethod]    
    public void TestRecurentTask_4() => TestRecurentTask_SumCoherence(ComponentIterator_Recursive_With_Branches());
    [TestMethod]    
    public void TestRecurentTask_5() => TestRecurentTask_SumCoherence(PartTypeIterator_Flat_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_6() => TestRecurentTask_SumCoherence(PartTypeIterator_Recursive_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_7() => TestRecurentTask_SumCoherence(PartTypeIterator_Flat_WithBranches());
    [TestMethod]    
    public void TestRecurentTask_8() => TestRecurentTask_SumCoherence(PartTypeIterator_Recursive_WithBranches());
    [TestMethod]    
    public void TestRecurentTask_9() => TestRecurentTask_SumCoherence(PartLocationIterator_Flat_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_10() => TestRecurentTask_SumCoherence(PartLocationIterator_Recursive_NoBranches());
    [TestMethod]    
    public void TestRecurentTask_11() => TestRecurentTask_SumCoherence(PartLocationIterator_Flat_WithBranches());
    [TestMethod]    
    public void TestRecurentTask_12() => TestRecurentTask_SumCoherence(PartLocationIterator_Recursive_WithBranches());

    //[TestMethod]
    //public void TestColumn_NonRecurentTaskTotalDuration()
    //{
    //    var part = new DecimalPropertyPartExemple<NonRecurrentTask>.Part_A();
    //    var i = new Pinstance(part);
    //    ColumnTester.TestPartTypeIteratorColumn_Decimal(
    //        i,
    //        TaskColumns.TaskTotalDuration(includeNonRecurent: true),
    //        i => i.Tasks()?.NonRecurentTasks ?? [],
    //        DecimalPropertyPartExemple.ExpectedTotal_IntensiveT);
    //}
}