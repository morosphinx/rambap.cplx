using SkiaSharp;

namespace rambap.cplx.Export.Prodocs.Drawings;

internal record PartBox
{
    public required string PN;
    public string PNSubDesc = "";
    public required string CN;

    public SKRect Expected_DrawPartBox
        => PNSubDesc != "" ? new SKRect(0, 0, 100, 84) : new SKRect(0, 0, 100, 75);

    public SKPoint DrawLinkAnchor_L => new SKPoint(0, Expected_DrawPartBox.Bottom - 30);
    public SKPoint DrawLinkAnchor_R => new SKPoint(100, Expected_DrawPartBox.Bottom - 30);
    public SKPoint DrawLinkAnchor_B => new SKPoint(50, Expected_DrawPartBox.Bottom);
}

internal static class Systems
{
    private const float Font_Title_Heigth = 12;
    private static SKFont Font_Title = new SKFont()
    {
        Size = Font_Title_Heigth,
        LinearMetrics = true, 
        
    };

    private const float Font_Comment_Heigth = 8;
    private static SKFont Font_Comment = new SKFont()
    {
        Size = Font_Comment_Heigth,
        LinearMetrics = true,
    };

    public static void DrawPartBox(this SKCanvas canvas, PartBox box, SKPoint topLeft)
    {
        SKPaint paintblack = new() { Color = 0xFF000000, StrokeWidth = 1};
        SKPaint paintblackStroke = new() { Color = 0xFF000000, StrokeWidth = 1, IsStroke = true };

        // Cursor advancing as we draw items, to to bottom
        SKPoint cursor = topLeft;
        void AdvanceCursor(float heigth)
        {
            cursor += new SKPoint(0, heigth);
        }
        void AddSpacer(int size = 1)
        {
            AdvanceCursor(size);
        }

        // Draw PN
        AdvanceCursor(Font_Title_Heigth);
        canvas.DrawText(box.PN, cursor, Font_Title, paintblack);
        AddSpacer();
        // Draw PNSubDesc
        if (box.PNSubDesc != "")
        {
            AdvanceCursor(Font_Comment_Heigth);
            canvas.DrawText(box.PNSubDesc, cursor, Font_Comment, paintblack);
            AddSpacer();
        }
        AddSpacer(2);

        // Draw MainBox
        SKRect mainBpx = new SKRect(0, 0, 100, 60);
        mainBpx.Offset(cursor);
        canvas.DrawRect(mainBpx, paintblackStroke);
        // Draw CN insideMainBox
        var boxCenter = new SKPoint(mainBpx.MidX, mainBpx.MidY + Font_Title_Heigth / 2);
        canvas.DrawText(box.CN, boxCenter, SKTextAlign.Center, Font_Title, paintblack);
        AdvanceCursor(mainBpx.Height);
        AddSpacer();
    }
}
