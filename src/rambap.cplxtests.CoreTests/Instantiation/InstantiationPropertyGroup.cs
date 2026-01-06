
namespace rambap.cplxtests.CoreTests.Instantiation;

[TestClass]
public class PropertyGroup_Simple : InstantiationCheckPart
{
    PropGroup Properties;

    class PropGroup : PropertyGroup
    {
        public EmptyPart PG_01;
        EmptyPart PG_02;
        EmptyPart PG_03 { get; set; }
    }
    public override IEnumerable<string> ExpectedCNs => [nameof(PropGroup.PG_01), "PG_02", "PG_03"];
}

[TestClass]
public class PropertyGroup_Nested : InstantiationCheckPart
{
    PropGroup_Top Properties;

    class PropGroup_Top : PropertyGroup
    {
        public EmptyPart PG_01;
        PropGroup_Deep Props;
    }
    class PropGroup_Deep : PropertyGroup
    {
        public EmptyPart PG_02;
        EmptyPart PG_03;
    }
    public override IEnumerable<string> ExpectedCNs =>
        [nameof(PropGroup_Top.PG_01), nameof(PropGroup_Deep.PG_02), "PG_03"];
}

[TestClass]
public class PropertyGroup_Multiples : InstantiationCheckPart
{
    PropGroup Group1;
    PropGroup Group2;

    class PropGroup : PropertyGroup
    {
        EmptyPart PG;
    }
    public override IEnumerable<string> ExpectedCNs => ["PG","PG"]; // Produces a duplicate CN
}