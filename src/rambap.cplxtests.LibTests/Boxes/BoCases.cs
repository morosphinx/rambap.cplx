using rambap.cplx.Modules.Costing;

namespace rambap.cplxtests.LibTests.Boxes;

// A fake series of electronic enclosures

[PN("BOC301005")]
public class BoCases_30_10_5 : Part
{
    Offer RS = new()
    {
        Supplier = "RadioS",
        SKU = "RS789-4562",
        Price = 45.48,
        Link = "www.radis.demo/789-4562"
    };
}

[PN("BOC301508")]
public class BoCases_30_15_8 : Part
{

}


[PN("BOC401510")]
public class BoCases_40_15_10 : Part
{

}