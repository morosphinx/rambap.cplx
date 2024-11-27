using SkiaSharp;

namespace rambap.cplx.Export.Plot;

// https://swharden.com/csdv/skiasharp/quickstart-console/
public class SkiaTest : IInstruction
{
    public void Do(string path)
    {
        // Create an image and fill it blue
        SKBitmap bmp = new(640, 480);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(SKColor.Parse("#003366"));

        // Draw lines with random positions and thicknesses
        Random rand = new(0);
        SKPaint paint = new() { Color = SKColors.White.WithAlpha(100), IsAntialias = true };
        for (int i = 0; i < 100; i++)
        {
            SKPoint pt1 = new(rand.Next(bmp.Width), rand.Next(bmp.Height));
            SKPoint pt2 = new(rand.Next(bmp.Width), rand.Next(bmp.Height));
            paint.StrokeWidth = rand.Next(1, 10);
            canvas.DrawLine(pt1, pt2, paint);
        }

        // Save the image to disk
        SKFileWStream fs = new(path);
        bmp.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
    }

}

