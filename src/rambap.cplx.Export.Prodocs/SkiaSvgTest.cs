using SkiaSharp;

namespace rambap.cplx.Export.Prodocs;

public class SkiaSvgTest
{
    public static void Test1()
    {
        SKRect rect = SKRect.Create(new SKSize(500, 500));
        using (SKFileWStream skstream = new("CsvText.svg"))
        {
            using (var canvas = SKSvgCanvas.Create(rect, skstream))
            {
                SKPaint paint = new() { Color = 0xFF125354};
                canvas.DrawCircle(50, 50, 25, paint);
                SKPaint paint2 = new() { Color = 0xFF125354, StrokeWidth = 1 };
                canvas.DrawText("This is a test", 80, 80, paint2);
            }
        }
    }
}
