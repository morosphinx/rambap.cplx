﻿using System.Xml.Serialization;

namespace rambap.cplx.UnitTests.CoreTests;



[TestClass]
public class ImplicitPartConstruction
{
    /// <summary>
    /// Test class with all different ways to declare a part as a component of another
    /// </summary>
    class TopLvlPart : Part
    {
        public MidLvlPart Mid_null_field;
        public MidLvlPart Mid_null_property { get; set; }

        public MidLvlPart Mid_auto_field = new();
        public MidLvlPart Mid_auto_property { get; set; } = new();

        public MidLvlPart Mid_constructed_field;
        public MidLvlPart Mid_constructed_property { get; init; }

        [CplxIgnore]
        public MidLvlPart Ignored_null_field;

        [CplxIgnore]
        public MidLvlPart Ignored_null_property;

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
            ];

        public static List<string> IgnoredComponents =>
            [
                nameof(Ignored_null_field),
                nameof(Ignored_null_property),
            ];
    }
    class MidLvlPart : Part { }

    /// <summary>
    /// Test that all ways to declare a component are supported
    /// </summary>
    [TestMethod]
    public void TestSingleComponentCreation()
    {
        var p = new TopLvlPart();
        var i = new Pinstance(p);
        foreach(var cn in TopLvlPart.ExpectedComponents)
        {
            Assert.IsTrue(i.Components.Any(c => c.CN == cn));
        }
        Assert.AreEqual(TopLvlPart.ExpectedComponents.Count, i.Components.Count());
    }

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
    /// Test that all ways to declare a component list are supported
    /// </summary>
    [TestMethod]
    public void TestEnumerableComponentCreation()
    {
        var p = new TopLvlPart_ListMode();
        var i = new Pinstance(p);
        foreach(var c in i.Components) Console.WriteLine(c.CN);
        // Number of subcomponent is valid
        Assert.AreEqual(TopLvlPart_ListMode.ExpectedTotalPartCount, i.Components.Count());
        // All SubComponents havz a distinct CN
        Assert.AreEqual(TopLvlPart_ListMode.ExpectedTotalPartCount, i.Components.Select(c => c.CN).Distinct().Count());
    }


    /// <summary>
    /// Test that component marked with <see cref="CplxIgnoreAttribute"/> are properly ignored
    /// </summary>
    [TestMethod]
    public void TestCplxIgnore()
    {
        var p = new TopLvlPart();
        var i = new Pinstance(p);
        foreach (var cn in TopLvlPart.IgnoredComponents)
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
        var p = new TopLvlPart();
        /// <see cref="Part.CplxImplicitInitialization"/> is run during instance construction
        var i = new Pinstance(p);
        // Test that Parents information has been properly set
        Assert.IsTrue(p.Mid_null_field.Parent == p);
        Assert.IsTrue(p.Mid_null_property.Parent == p);
        Assert.IsTrue(p.Mid_auto_field.Parent == p);
        Assert.IsTrue(p.Mid_auto_property.Parent == p);
        Assert.IsTrue(p.Mid_constructed_field.Parent == p);
        Assert.IsTrue(p.Mid_constructed_property.Parent == p);
    }
}