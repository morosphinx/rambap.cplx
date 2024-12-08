using System.Xml.Serialization;

namespace rambap.cplx.UnitTests.CoreTests;



[TestClass]
public class ImplicitPartConstruction
{
    /// <summary>
    /// Test class with all different ways to declare a part as a component of another
    /// </summary>
    class ParentPart : Part
    {
        public ChildPart Child_null_field;
        public ChildPart Child_null_property { get; set; }

        public ChildPart Child_auto_field = new();
        public ChildPart Child_auto_property { get; set; } = new();

        public ChildPart Child_constructed_field;
        public ChildPart Child_constructed_property { get; init; }

        [CplxIgnore]
        public ChildPart Ignored_null_field;

        [CplxIgnore]
        public ChildPart Ignored_null_property;

        public ParentPart()
        {
            Child_constructed_field = new ChildPart();
            Child_constructed_property = new ChildPart();
        }

        public static List<string> ExpectedComponents =>
            [
                nameof(Child_null_field),
                nameof(Child_null_property),
                nameof(Child_auto_field),
                nameof(Child_auto_property),
                nameof(Child_constructed_field),
                nameof(Child_constructed_property),
            ];

        public static List<string> IgnoredComponents =>
            [
                nameof(Ignored_null_field),
                nameof(Ignored_null_property),
            ];
    }
    class ChildPart : Part { }

    /// <summary>
    /// Test that all ways to declare a component are supported
    /// </summary>
    [TestMethod]
    public void TestSingleComponentCreation()
    {
        var p = new ParentPart();
        var i = new Pinstance(p);
        foreach(var cn in ParentPart.ExpectedComponents)
        {
            Assert.IsTrue(i.Components.Any(c => c.CN == cn));
        }
        Assert.AreEqual(ParentPart.ExpectedComponents.Count, i.Components.Count());
    }

    /// <summary>
    /// Test that all ways to declare a component list are supported
    /// </summary>
    [TestMethod]
    public void TestEnumerableComponentCreation()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Test that component marked with <see cref="CplxIgnoreAttribute"/> are properly ignored
    /// </summary>
    [TestMethod]
    public void TestCplxIgnore()
    {
        var p = new ParentPart();
        var i = new Pinstance(p);
        foreach (var cn in ParentPart.IgnoredComponents)
        {
            Assert.IsFalse(i.Components.Any(c => c.CN == cn));
        }
    }

    /// <summary>
    /// Test that the <see cref="Part.Parent"/> property is set
    /// </summary>
    [TestMethod]
    public void TestParentRelation()
    {
        var p = new ParentPart();
        /// <see cref="Part.CplxImplicitInitialization"/> is run during instance construction
        var i = new Pinstance(p);
        // Test that Parents information has been properly set
        Assert.IsTrue(p.Child_null_field.Parent == p);
        Assert.IsTrue(p.Child_null_property.Parent == p);
        Assert.IsTrue(p.Child_auto_field.Parent == p);
        Assert.IsTrue(p.Child_auto_property.Parent == p);
        Assert.IsTrue(p.Child_constructed_field.Parent == p);
        Assert.IsTrue(p.Child_constructed_property.Parent == p);
    }
}
