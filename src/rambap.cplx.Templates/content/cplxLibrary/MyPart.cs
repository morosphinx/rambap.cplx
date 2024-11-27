namespace rambap.cplx.Template.Lib;

public class MyPart : Part
{
    [ComponentDescription("First component")]
    MySubPart C01 { get; set; }

    // Use in-code documentation, and links : https://en.wikipedia.org/wiki/System
    [ComponentDescription("Second component")]
    MySubPart C02 { get; set; }

    Cost AssemblyCost = 550;
}

[PN("15678-0545")]
[PartDescription("SubComponent of the demo part")]
public class MySubPart : Part
{
    Cost Buy = 4000.45m;
}