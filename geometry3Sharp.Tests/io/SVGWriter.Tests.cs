using g3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace geometry3Sharp.Tests.io
{
    [TestClass]
    public class SVGWriterTests
    {
        [TestMethod]
        public void TestBasic()
        {
            var writer = new SVGWriter();
            writer.AddLine(new Segment2d(Vector2d.Zero, Vector2d.AxisX));
            writer.Write("basic.svg");
        }

        [TestMethod]
        public void TestLayers()
        {
            var writer = new SVGWriter();
            writer.StartNewLayer("layer-0");
            writer.AddLine(new Segment2d(Vector2d.Zero, Vector2d.AxisX));

            writer.StartNewLayer("layer-1");
            writer.AddLine(new Segment2d(Vector2d.Zero, Vector2d.AxisY));
            writer.Write("layers.svg");
        }

        [TestMethod]
        public void TestOpacity()
        {
            var writer = new SVGWriter();

            writer.AddPolygon(Polygon2d.MakeRectangle(Vector2d.Zero, 6, 12), new SVGWriter.Style()
            {
                fill = "blue",
                fillOpacity = 0.3f,
                stroke = "blue",
                stroke_width = 0.1f
            });

            writer.AddPolygon(Polygon2d.MakeRectangle(Vector2d.Zero, 12, 6), new SVGWriter.Style()
            {
                fill = "red",
                fillOpacity = 0.3f,
                stroke = "red",
                stroke_width = 0.1f
            });

            writer.Write("opacity.svg");
        }

        [TestMethod]
        public void TestGeneralPolygon2d()
        {
            var writer = new SVGWriter();

            var polyWithHoles = new GeneralPolygon2d(Polygon2d.MakeRectangle(Vector2d.Zero, 10, 20));
            var hole1 = Polygon2d.MakeCircle(3, 16).Translate(new Vector2d(1, 5));
            var hole2 = Polygon2d.MakeCircle(3, 16).Translate(new Vector2d(-1, -5));
            hole1.Reverse();
            hole2.Reverse();
            polyWithHoles.AddHole(hole1);
            polyWithHoles.AddHole(hole2);

            var style = SVGWriter.Style.Filled("blue", "blue", 0.1f);
            style.fillOpacity = 0.3f;
            writer.AddPolygon(polyWithHoles, style);

            writer.Write("holes.svg");
        }
    }
}