namespace rambap.cplxtests.LibTests.Raspberry;

[PartDescription("Raspberry Pi 5")]
public abstract class Pi5 : Part
{
    // I couldn't find an official source on rasberry pi5 port names
    Link Datasheet = "https://www.raspberrypi.com/documentation/computers/raspberry-pi.html";

    public Pi5_Header GPIO_Header;

    public ConnectablePort MicroHDM1, MicroHDMI2;

    public ConnectablePort USB2_1, USB2_2, USB3_1, USB3_2;

    public ConnectablePort USBC_Power;

    public ConnectablePort EThernet;
}

[PN("SC1110")]
[PartDescription("Raspberry Pi 5 / 2Gb")]
public class Pi5_2Gb : Pi5
{
    Offer Mouser = new()
    {
        Price = 48,
        Link = "https://www.mouser.fr/ProductDetail/Raspberry-Pi/SC1110?qs=IKkN%2F947nfD9lkt4mJ3PfA%3D%3D",
        SKU = "358-SC1110"
    };
}

[PN("SC1111")]
[PartDescription("Raspberry Pi 5 / 4Gb")]
public class Pi5_4Gb : Pi5
{
    Offer Mouser = new()
    {
        Price = 57,
        Link = "https://www.mouser.fr/ProductDetail/Raspberry-Pi/SC1111?qs=HoCaDK9Nz5fnLhlMNnKTiQ%3D%3D",
        SKU = "358-SC1111"
    };
}

[PN("SC1112")]
[PartDescription("Raspberry Pi 5 / 8Gb")]
public class Pi5_8Gb : Pi5
{
    Offer Mouser = new()
    {
        Price = 76,
        Link = "https://www.mouser.fr/ProductDetail/Raspberry-Pi/SC1112?qs=HoCaDK9Nz5c86n0i5EQ%2FPA%3D%3D",
        SKU = "358-SC1112"
    };
}

[PN("SC1113")]
[PartDescription("Raspberry Pi 5 / 16Gb")]
public class Pi5_16Gb : Pi5
{
    Offer Mouser = new()
    {
        Price = 114,
        Link = "https://www.mouser.fr/ProductDetail/Raspberry-Pi/SC1113?qs=a2MtRaTmNOQZp5XJGfiB8A%253D%253D",
        SKU = "358-SC1113"
    };
}