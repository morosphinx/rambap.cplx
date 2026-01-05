using rambap.cplx.Core;
using rambap.cplx.Modules.Connectivity.PinstanceModel;

namespace rambap.cplxtests.CoreTests;

[TestClass]
public class ImplicitPartConstruction
{
    /// <summary>
    /// Test class with all different ways to declare a part as a component of another, no inheritance
    /// </summary>
    class TopLvlPart : Part
    {
        public MidLvlPart Mid_null_field;
        public MidLvlPart Mid_null_property { get; set; }

        public MidLvlPart Mid_auto_field = new();
        public MidLvlPart Mid_auto_property { get; set; } = new();

        public MidLvlPart Mid_constructed_field;
        public MidLvlPart Mid_constructed_property { get; init; }

        public MidLvlPart Mid_inlineConstructed_Property { get; } = new();

        [CplxIgnore]
        public MidLvlPart Ignored_null_field;

        [CplxIgnore]
        public MidLvlPart Ignored_null_property;

        public MidLvlPart PartAlias => Ignored_null_field;

        public TopLvlPart()
        {
            Mid_constructed_field = new MidLvlPart();
            Mid_constructed_property = new MidLvlPart();
        }

        public static List<string> ExpectedComponents =>
            [
                nameof(Mid_null_field),
                nameof(Mid_null_property),
                nameof(Mid_auto_field),
                nameof(Mid_auto_property),
                nameof(Mid_constructed_field),
                nameof(Mid_constructed_property),
                nameof(Mid_inlineConstructed_Property),
            ];

        public static List<string> NotExpectedComponents =>
            [
                nameof(Ignored_null_field),
                nameof(Ignored_null_property),
                nameof(PartAlias),
            ];
    }
    class MidLvlPart : Part { }

    
    class PropertyGroup1 : PropertyGroup
    {
        public MidLvlPart PG11;
        public MidLvlPart PG12;
    }
    class PropertyGroup2 : PropertyGroup
    {
        public MidLvlPart PG21;
        public MidLvlPart PG22;
        PropertyGroup3 G3;
    }
    class PropertyGroup3 : PropertyGroup
    {
        public MidLvlPart PG31;
        public MidLvlPart PG32;
    }
    class TopLevelPartWithPropertyGroups : Part
    {
        PropertyGroup1 G1;
        PropertyGroup2 G2;
        public static List<string> ExpectedComponents =>
            [
                nameof(PropertyGroup1.PG11),
                nameof(PropertyGroup1.PG12),
                nameof(PropertyGroup2.PG21),
                nameof(PropertyGroup2.PG22),
                nameof(PropertyGroup3.PG31),
                nameof(PropertyGroup3.PG32),
            ];
    }


    /// <summary>
    /// Test classes with all different ways to declare a part as a component of another, with inheritance
    /// </summary>
    class ParentPart : Part
    {
        MidLvlPart PrivHiddenField;
        MidLvlPart PrivHiddenProp { get; } = new();
    }
    class ChildPart : ParentPart
    {
        MidLvlPart PrivHiddenField;
        MidLvlPart PrivHiddenProp { get; } = new();
        MidLvlPart ErrorTrigger => null;

        public const int ExpectedComponentCount = 4; // Both parent and child should be here, no collision
    }






    /// <summary>
    /// Test that all ways to declare a component are supported
    /// </summary>
    [TestMethod]
    public void TestSingleComponentCreation()
    {
        var part = new TopLvlPart();
        var component = part.Instantiate();
        foreach (var subcomp_cn in TopLvlPart.ExpectedComponents)
        {
            Assert.IsTrue(component.SubComponents.Any(c => c.CN == subcomp_cn));
        }
        Assert.AreEqual(TopLvlPart.ExpectedComponents.Count, component.SubComponents.Count());
    }

    [TestMethod]
    public void TestInheritanceomponentCreation()
    {
        var part = new ChildPart();
        var component = part.Instantiate();
        Assert.AreEqual(ChildPart.ExpectedComponentCount, component.SubComponents.Count());
    }


    [TestMethod]
    public void TestPropertyGroupComponentCreation()
    {
        var part = new TopLevelPartWithPropertyGroups();
        var component = part.Instantiate();
        foreach (var subcomp_cn in TopLevelPartWithPropertyGroups.ExpectedComponents)
        {
            Assert.IsTrue(component.SubComponents.Any(c => c.CN == subcomp_cn));
        }
        Assert.AreEqual(TopLevelPartWithPropertyGroups.ExpectedComponents.Count, component.SubComponents.Count());
    }

    /// <summary>
    /// Test that component marked with <see cref="CplxIgnoreAttribute"/> are properly ignored
    /// </summary>
    [TestMethod]
    public void TestCplxIgnore()
    {
        var part = new TopLvlPart();
        var component = part.Instantiate();
        foreach (var cn in TopLvlPart.NotExpectedComponents)
        {
            Assert.IsFalse(component.SubComponents.Any(c => c.CN == cn));
        }
    }

    /// <summary>
    /// Test that the <see cref="Part.Parent"/> property is set
    /// </summary>
    [TestMethod]
    public void TestParentRelation()
    {
        var part = new TopLvlPart();
        /// <see cref="Part.CplxImplicitInitialization"/> is run during instance construction
        var instance = part.Instantiate().Instance;
        // Test that Parents information has been properly set
        Assert.IsTrue(part.Mid_null_field.Parent == part);
        Assert.IsTrue(part.Mid_null_property.Parent == part);
        Assert.IsTrue(part.Mid_auto_field.Parent == part);
        Assert.IsTrue(part.Mid_auto_property.Parent == part);
        Assert.IsTrue(part.Mid_constructed_field.Parent == part);
        Assert.IsTrue(part.Mid_constructed_property.Parent == part);
    }
}
