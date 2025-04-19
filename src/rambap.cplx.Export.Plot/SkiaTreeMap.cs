using SkiaSharp;
using System.Drawing;
using rambap.cplx.Core;

namespace rambap.cplx.Export.Plot;

// https://swharden.com/blog/2023-03-07-treemapping/
public class SkiaTreeMap : IInstruction
{
    private Component Content;
    private static List<(string CN, decimal cost)> ListNativeCosts(string CN, Component compo)
    {
        List<(string, decimal)> costs = new();
        if (i.Cost()?.Native > 0)
            costs.Add(($"{CN}\n{compo.PN}", compo.Instance.Cost()?.Native ?? 0));
        foreach(var c in compo.SubComponents)
            if(c.Instance.Cost()?.Total > 0)
                costs.AddRange(ListNativeCosts(c.CN, compo));
        return costs;
    }

    public SkiaTreeMap(Component content)
    {
        Content = content ;
    }

    public void Do(string path)
    {
        var list = ListNativeCosts("Root", Content).OrderByDescending(i => i.cost).ToList();
        double[] sortedValues = list.Select(i => Decimal.ToDouble(i.cost)).ToArray();
        string[] labels = list.Select(i => i.CN).ToArray();

        // Calculate the size and position of all rectangles in the tree map
        int width = 600;
        int height = 400;
        RectangleF[] rectangles = TreeMap.GetRectangles(sortedValues, width, height);

        // Create an image to draw on (with 1px extra to make room for the outline)
        using SKBitmap bmp = new(width + 1, height + 1);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(SKColors.White);

        // Draw and label each rectangle
        for (int i = 0; i < rectangles.Length; i++)
        {
            SKPaint fill = new()
            {
                Color = new SKColor(
                    red : (byte) Random.Shared.Next(150, 250),
                    green: (byte) Random.Shared.Next(150, 250),
                    blue: (byte) Random.Shared.Next(150, 250)
                ),
                Style = SKPaintStyle.Fill
            };
            SKPaint borders = new SKPaint()
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
            };
            SKPaint fontcolor = new SKPaint()
            {
                Color = SKColors.Black,
            };
            int fontsize = 12;
            SKFont font = new SKFont()
            {
                Size = fontsize,
                Typeface = SKTypeface.FromFamilyName("Consolas")
            };

            var rect = rectangles[i];
            var label = labels[i];
            SKRect skrect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
            canvas.DrawRect(skrect, fill);
            canvas.DrawRect(skrect, borders);
            int lineindex = 1;
            foreach(var line in label.Split("\n"))
                canvas.DrawText(line, skrect.Left + 4, skrect.Top + (fontsize * lineindex++), font, fontcolor);
        }

        // Save the image to disk
        SKFileWStream fs = new(path);
        bmp.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
    }

    internal static class TreeMap
    {
        public static RectangleF[] GetRectangles(double[] values, int width, int height)
        {
            for (int i = 1; i < values.Length; i++)
                if (values[i] > values[i - 1])
                    throw new ArgumentException("values must be ordered large to small");

            var slice = GetSlice(values, 1, 0.35);
            var rectangles = GetRectangles(slice, width, height);
            return rectangles.Select(x => x.ToRectF()).ToArray();
        }

        private class Slice
        {
            public double Size { get; }
            public IEnumerable<double> Values { get; }
            public Slice[] Children { get; }

            public Slice(double size, IEnumerable<double> values, Slice sub1, Slice sub2)
            {
                Size = size;
                Values = values;
                Children = new Slice[] { sub1, sub2 };
            }

            public Slice(double size, double finalValue)
            {
                Size = size;
                Values = new double[] { finalValue };
                Children = Array.Empty<Slice>();
            }
        }

        private class SliceResult
        {
            public double ElementsSize { get; }
            public IEnumerable<double> Elements { get; }
            public IEnumerable<double> RemainingElements { get; }

            public SliceResult(double elementsSize, IEnumerable<double> elements, IEnumerable<double> remainingElements)
            {
                ElementsSize = elementsSize;
                Elements = elements;
                RemainingElements = remainingElements;
            }
        }

        private class SliceRectangle
        {
            public Slice Slice { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public SliceRectangle(Slice slice) => Slice = slice;
            public RectangleF ToRectF() => new(X, Y, Width, Height);
        }

        private static Slice GetSlice(IEnumerable<double> elements, double totalSize, double sliceWidth)
        {
            if (elements.Count() == 1)
                return new Slice(totalSize, elements.Single());

            SliceResult sr = GetElementsForSlice(elements, sliceWidth);
            Slice child1 = GetSlice(sr.Elements, sr.ElementsSize, sliceWidth);
            Slice child2 = GetSlice(sr.RemainingElements, 1 - sr.ElementsSize, sliceWidth);
            return new Slice(totalSize, elements, child1, child2);
        }

        private static SliceResult GetElementsForSlice(IEnumerable<double> elements, double sliceWidth)
        {
            var elementsInSlice = new List<double>();
            var remainingElements = new List<double>();
            double current = 0;
            double total = elements.Sum();

            foreach (var element in elements)
            {
                if (current > sliceWidth)
                    remainingElements.Add(element);
                else
                {
                    elementsInSlice.Add(element);
                    current += element / total;
                }
            }

            return new SliceResult(current, elementsInSlice, remainingElements);
        }

        private static IEnumerable<SliceRectangle> GetRectangles(Slice slice, int width, int height)
        {
            SliceRectangle area = new(slice) { Width = width, Height = height };

            foreach (var rect in GetRectangles(area))
            {
                if (rect.X + rect.Width > area.Width)
                    rect.Width = area.Width - rect.X;

                if (rect.Y + rect.Height > area.Height)
                    rect.Height = area.Height - rect.Y;

                yield return rect;
            }
        }

        private static IEnumerable<SliceRectangle> GetRectangles(SliceRectangle sliceRectangle)
        {
            var isHorizontalSplit = sliceRectangle.Width >= sliceRectangle.Height;
            var currentPos = 0;
            foreach (var subSlice in sliceRectangle.Slice.Children)
            {
                var subRect = new SliceRectangle(subSlice);
                int rectSize;

                if (isHorizontalSplit)
                {
                    rectSize = (int)Math.Round(sliceRectangle.Width * subSlice.Size);
                    subRect.X = sliceRectangle.X + currentPos;
                    subRect.Y = sliceRectangle.Y;
                    subRect.Width = rectSize;
                    subRect.Height = sliceRectangle.Height;
                }
                else
                {
                    rectSize = (int)Math.Round(sliceRectangle.Height * subSlice.Size);
                    subRect.X = sliceRectangle.X;
                    subRect.Y = sliceRectangle.Y + currentPos;
                    subRect.Width = sliceRectangle.Width;
                    subRect.Height = rectSize;
                }

                currentPos += rectSize;

                if (subSlice.Values.Count() > 1)
                {
                    foreach (var sr in GetRectangles(subRect))
                    {
                        yield return sr;
                    }
                }
                else if (subSlice.Values.Count() == 1)
                {
                    yield return subRect;
                }
            }
        }
    }
}

