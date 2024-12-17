namespace rambap.cplx.UnitTests.ModuleTests;

[TestClass]
public class CostingTests
{
    public class CostTestPart : Part
    {
        Cost C1;
        Cost C2 = 1;
        Cost C3 = 2.0;
        Cost C4 = 4m;
    }
    public class RecuringTaskTestPart : Part
    {
        RecurrentTask T1;
        RecurrentTask T2 = 1;
        RecurrentTask T3 = 2.0;
        RecurrentTask T4 = 4m;

        RecurrentTask T5 = (8, TaskCategory.Validation);
        RecurrentTask T6 = (16.0, TaskCategory.Validation);
        RecurrentTask T7 = (32m, TaskCategory.Validation);

        RecurrentTask T8 = new(64,"Validation");
    }

    public class NonRecurrentTaskTestPart : Part
    {
        NonRecurrentTask T1;
        NonRecurrentTask T2 = 1;
        NonRecurrentTask T3 = 2.0;
        NonRecurrentTask T4 = 4m;

        NonRecurrentTask T5 = (8, TaskCategory.Validation);
        NonRecurrentTask T6 = (16.0, TaskCategory.Validation);
        NonRecurrentTask T7 = (32m, TaskCategory.Validation);

        NonRecurrentTask T8 = new(64, "Validation");
    }
}
