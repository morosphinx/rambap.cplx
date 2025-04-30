using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplxtests.LibTests.Connectors;

// A fake serie of 2mm Pannel mount sockets
// Loosely copied from multicomp 24.102.1 2mm mount socket

public abstract class TestSocket2mm : Part, IPartConnectable
{
    public enum ColorOptions
    {
        Red,
        Black,
    }

    public ConnectablePort Socket2mm;
    public WireablePort WiringTab;

    Manufacturer SocketMaker;
    Link Datasheet = "www.SocketMaker.demo/products/2mmSystem/24102";

    internal TestSocket2mm(ColorOptions color)
    {
        PN = $"24.102.{(int)color + 1}";
        CommonName = $"Test Socket 2mm, {color}";
    }

    public void Assembly_Connections(ConnectionBuilder Do)
    {
        Do.StructuralConnection(Socket2mm, WiringTab);
    }
}

public class TestSocket2mm_Red : TestSocket2mm
{
    public TestSocket2mm_Red() : base(ColorOptions.Red){}

    Offer RadioS = new()
    {
        SKU = "RS 893-4574",
        Price = 2.56,
        Link = "www.radis.demo/893-4574"
    };
}

public class TestSocket2mm_Black : TestSocket2mm
{
    public TestSocket2mm_Black() : base(ColorOptions.Black) { }

    Offer RadioS = new()
    {
        SKU = "RS 893-4579",
        Price = 2.82,
        Link = "www.radis.demo/893-4579"
    };
}