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
    /// Test classes with all different ways to declare a lsit of components
    /// </summary>
    class TopLvlPart_ListMode : Part
    {
        public List<MidLvlPart> MidParts_auto_field = [
                new MidLvlPart(),
                new MidLvlPart(),
                new MidLvlPart(),
            ];

        public List<MidLvlPart> MidParts_auto_property = [
                new MidLvlPart(),
                new MidLvlPart(),
            ];

        public List<MidLvlPart> MidParts_constructed_field;
        public List<MidLvlPart> MidParts_constructed_property;

        public TopLvlPart_ListMode()
        {
            MidParts_constructed_field = [new MidLvlPart(), new MidLvlPart()];
            MidParts_constructed_property = [new MidLvlPart()];

            AdditionalComponents.Add(new MidLvlPart() { CN = "autoPart1" });
            AdditionalComponents.Add(new MidLvlPart() { CN = "autoPart2" });
            AdditionalComponents.Add(new MidLvlPart() { CN = "autoPart3" });
            AdditionalComponents.Add(new MidLvlPart() { CN = "autoPart4" });
        }

        public static int ExpectedTotalPartCount = 3 + 2 + 2 + 1 + 4;
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

    /// <summary>
    /// Test that all ways to declare a component list are supported
    /// </summary>
    [TestMethod]
    public void TestEnumerableComponentCreation()
    {
        var part = new TopLvlPart_ListMode();
        var component = part.Instantiate();
        foreach (var subcomp in component.SubComponents) Console.WriteLine(subcomp.CN);
        // Number of subcomponent is valid
        Assert.AreEqual(TopLvlPart_ListMode.ExpectedTotalPartCount, component.SubComponents.Count());
        // All SubComponents havz a distinct CN
        Assert.AreEqual(TopLvlPart_ListMode.ExpectedTotalPartCount, component.SubComponents.Select(c => c.CN).Distinct().Count());
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
