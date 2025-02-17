using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;
using rambap.cplx.Modules.Costing.Outputs;
using rambap.cplx.Modules.Costing;

namespace rambap.cplx.UnitTests.ExportValidity;


[TestClass]
public class TestColumn_Costs : TestColumn_ExtensiveProperty<Cost, InstanceCost.NativeCostInfo>
{
    protected override IEnumerable<InstanceCost.NativeCostInfo> PropertyIterator(Component component)
        => component.Instance.Cost()?.NativeCosts ?? [];

    protected override IColumn<IComponentContent> GetTestedColumn()
        => CostColumns.TotalCost();

    protected override string PropertyNaming(IPropertyContent<InstanceCost.NativeCostInfo> instanceProperty)
        => instanceProperty.Property.name;

    protected override IEnumerable<IColumn<IComponentContent>> GetDebugColumns()
        => [
                CostColumns.CostName(),
                CostColumns.UnitCost(),
           ];
}

[TestClass]
public class TestColumn_NonRecurentTask : TestColumn_ExtensiveProperty<RecurrentTask, InstanceTasks.NamedTask>
{
    protected override IEnumerable<InstanceTasks.NamedTask> PropertyIterator(Component component)
        => component.Instance.Tasks()?.RecurentTasks ?? [];

    protected override IColumn<IComponentContent> GetTestedColumn()
        => TaskColumns.TaskTotalDuration(false);

    protected override string PropertyNaming(IPropertyContent<InstanceTasks.NamedTask> instanceProperty)
        => instanceProperty.Property.Name;

    protected override IEnumerable<IColumn<IComponentContent>> GetDebugColumns()
        => [
                TaskColumns.TaskName(),
                TaskColumns.TaskCategory(),
           ];
}

// TODO : Test non recurent cost

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
