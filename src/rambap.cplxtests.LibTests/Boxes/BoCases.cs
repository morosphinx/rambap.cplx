using rambap.cplx.Modules.Costing;

namespace rambap.cplxtests.LibTests.Boxes;

// A fake series of electronic enclosures

[PN("BOC301005")]
[CommonName("Enclosure 30 x 10 x 5 cm")]
public class BoCases_30_10_5 : Part
{
    Offer RS = new()
    {
        Supplier = "RadioS",
        SKU = "RS789-4562",
        Price = 45.48,
        Link = "www.radis.demo/789-4562"
    };

    Manufacturer Bocases;
}

[PN("BOC301508")]
[CommonName("Enclosure 30 x 15 x 8 cm")]
public class BoCases_30_15_8 : Part
{
    Offer RS = new()
    {
        Supplier = "RadioS",
        SKU = "RS789-7862",
        Price = 59.45,
        Link = "www.radis.demo/789-7862"
    };

    Manufacturer Bocases;
}

[PN("BOC401510")]
[CommonName("Enclosure 40 x 15 x 10 cm")]
public class BoCases_40_15_10 : Part
{
    Offer RS = new()
    {
        Supplier = "RadioS",
        SKU = "RS789-1676",
        Price = 70.14,
        Link = "www.radis.demo/789-1676"
    };

    Manufacturer Bocases;
}