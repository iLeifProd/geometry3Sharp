using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	// ---------- Lines intersections aggregator
	public class LineIntersector : IntersectorBase<Line2d>
	{
		public LineIntersector(Line2d me) : base(me) { }
		public LineIntersector(Line2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, seg.P0, seg.P1 - seg.P0, Limits.Infinity, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Line2d line) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, line.Origin, line.Direction, Limits.Infinity, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Ray2d ray) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, ray.Origin, ray.Direction, Limits.Infinity, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Arc2d arc) =>
			ArcUtils2d.FindIntersect(Me.Origin, Me.Direction, arc, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Circle2d circle) =>
			CircleUtils2d.FindIntersect(Me.Origin, Me.Direction, circle, Limits.Infinity, Tolerance);
	}

	// ---------- Ray intersections aggregator
	public class RayIntersector : IntersectorBase<Ray2d>
	{
		public RayIntersector(Ray2d me) : base(me) { }
		public RayIntersector(Ray2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, seg.P0, seg.P1 - seg.P0, Limits.StartToInfinity, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Line2d line) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, line.Origin, line.Direction, Limits.StartToInfinity, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Ray2d ray) =>
			LineUtils2d.FindIntersect(Me.Origin, Me.Direction, ray.Origin, ray.Direction, Limits.StartToInfinity, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Arc2d arc) =>
			ArcUtils2d.FindIntersect(Me.Origin, Me.Direction, arc, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Circle2d circle) =>
			CircleUtils2d.FindIntersect(Me.Origin, Me.Direction, circle, Limits.StartToInfinity, Tolerance);
	}

	// ---------- Segment intersections aggregator
	public class SegmentIntersector2d : IntersectorBase<Segment2d>
	{
		public SegmentIntersector2d(Segment2d me) : base(me) { }
		public SegmentIntersector2d(Segment2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) =>
			LineUtils2d.FindIntersect(Me.P0, Me.P1 - Me.P0, seg.P0, seg.P1 - seg.P0, Limits.StartToEnd, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Line2d line) =>
			LineUtils2d.FindIntersect(Me.P0, Me.P1 - Me.P0, line.Origin, line.Direction, Limits.StartToEnd, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Ray2d ray) =>
			LineUtils2d.FindIntersect(Me.P0, Me.P1 - Me.P0, ray.Origin, ray.Direction, Limits.StartToEnd, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Arc2d arc) =>
			ArcUtils2d.FindIntersect(Me.P0, Me.P1 - Me.P0, arc, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Circle2d circle) =>
			CircleUtils2d.FindIntersect(Me.P0, Me.P1 - Me.P0, circle, Limits.StartToEnd, Tolerance);
	}

	// ---------- Arc intersections aggregator
	public class ArcIntersector2d : IntersectorBase<Arc2d>
	{
		public ArcIntersector2d(Arc2d me) : base(me) { }
		public ArcIntersector2d(Arc2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) =>
			ArcUtils2d.FindIntersect(seg.P0, seg.P1 - seg.P0, Me, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Line2d line) =>
			ArcUtils2d.FindIntersect(line.Origin, line.Direction, Me, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Ray2d ray) =>
			ArcUtils2d.FindIntersect(ray.Origin, ray.Direction, Me, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Arc2d arc) =>
			ArcUtils2d.FindIntersect(Me, arc, Tolerance);
		public override IntersectionResult2d IntersectWith(Circle2d circle) =>
			ArcUtils2d.FindIntersect(Me, circle, Tolerance);
	}

	// ---------- Circle intersections aggregator
	public class CircleIntersector2d : IntersectorBase<Circle2d>
	{
		public CircleIntersector2d(Circle2d me) : base(me) { }
		public CircleIntersector2d(Circle2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) =>
			CircleUtils2d.FindIntersect(seg.P0, seg.P1 - seg.P0, Me, Limits.StartToEnd, Tolerance);
		public override IntersectionResult2d IntersectWith(Line2d line) =>
			CircleUtils2d.FindIntersect(line.Origin, line.Direction, Me, Limits.Infinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Ray2d ray) =>
			CircleUtils2d.FindIntersect(ray.Origin, ray.Direction, Me, Limits.StartToInfinity, Tolerance);
		public override IntersectionResult2d IntersectWith(Arc2d arc) =>
			ArcUtils2d.FindIntersect(arc, Me, Tolerance);
		public override IntersectionResult2d IntersectWith(Circle2d circle) =>
			CircleUtils2d.FindIntersect(Me, circle, Tolerance);
	}

	// ---------- PolyCurve intersections aggregator
	public class PolyCurveIntersector2d : IntersectorBase<IMultiCurve2d>
	{
		public PolyCurveIntersector2d(IMultiCurve2d me) : base(me) { }
		public PolyCurveIntersector2d(IMultiCurve2d me, double tolerance) : base(me, tolerance) { }

		public override IntersectionResult2d IntersectWith(Segment2d seg) => PolyCurveUtils.FindIntersect(Me, seg);
		public override IntersectionResult2d IntersectWith(Line2d line) => PolyCurveUtils.FindIntersect(Me, line);
		public override IntersectionResult2d IntersectWith(Ray2d ray) => PolyCurveUtils.FindIntersect(Me, ray);
		public override IntersectionResult2d IntersectWith(Arc2d arc) => PolyCurveUtils.FindIntersect(Me, arc);
		public override IntersectionResult2d IntersectWith(Circle2d circle) => PolyCurveUtils.FindIntersect(Me, circle);
	}
}
