using rambap.cplx.Core;
using rambap.cplx.Export.Prodocs.Drawings;
using SkiaSharp;

namespace rambap.cplx.Export.Prodocs;

public class SvgSystemTree : IInstruction
{
    public required Pinstance Content { get; init; }

    public void Do(string path)
    {
        // Play drawing instruction in a recorder canvas in order to get the bounding rect output from the drawing method 
        var recorder = new SKPictureRecorder();
        var recorderCanvas = recorder.BeginRecording(new SKRect(0, 0, 10000, 10000));
        DrawSystemTree(recorderCanvas, out SKRect bounds);
        var record = recorder.EndRecording();

        using (SKFileWStream skstream = new(path))
        using (var canvas = SKSvgCanvas.Create(bounds, skstream))
        {
            record.Playback(canvas);
        }
    }

    private void DrawSystemTree(SKCanvas canvas, out SKRect boundingRect)
    {
        // Draw Main box
        var rootboxPos = new SKPoint(10, 10);
        var rootBox = new PartBox()
        {
            PN = Content.PN,
            PNSubDesc = Content.CommonName,
            CN = "*"
        };
        canvas.DrawPartBox(rootBox, rootboxPos);
        var llinkanchor = rootBox.DrawLinkAnchor_R + rootboxPos;
        // Draw Subcomponent boxes
        float vcursor = 20;
        foreach (var c in Content.Components)
        {
            // Draw SubBox
            var subcompBoxPos = new SKPoint(300, vcursor);
            var subcompBox = new PartBox()
            {
                PN = c.Instance.PN,
                PNSubDesc = c.Instance.CommonName,
                CN = c.CN,
            };
            canvas.DrawPartBox(subcompBox, subcompBoxPos);
            var rlinkanchor = subcompBox.DrawLinkAnchor_L + subcompBoxPos;
            vcursor += subcompBox.Expected_DrawPartBox.Height + 10;
            // DrawLink
            SKPaint paintblackStroke = new() { Color = 0xFF000000, StrokeWidth = 1, IsStroke = true };
            canvas.DrawLine(llinkanchor, rlinkanchor, paintblackStroke);
        }
        boundingRect = new SKRect(0, 0, 410, vcursor);
    }
}
