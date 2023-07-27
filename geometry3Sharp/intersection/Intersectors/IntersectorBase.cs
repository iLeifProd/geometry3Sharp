using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using g3.Intersections;

namespace g3.Intersections
{
	public abstract class IntersectorBase<TCurve> where TCurve : IIntersectionItem2d
	{
		public TCurve Me { get; set; }
		public double Tolerance { get; set; } = MathUtil.ZeroTolerance;

		protected IntersectorBase(TCurve me)
		{
			Me = me;
		}

		protected IntersectorBase(TCurve me, double tolerance)
		{
			Me = me;
			Tolerance = tolerance;
		}

		public abstract IntersectionResult2d IntersectWith(Segment2d seg);
		public abstract IntersectionResult2d IntersectWith(Arc2d arc);
		public abstract IntersectionResult2d IntersectWith(Circle2d circle);
		public abstract IntersectionResult2d IntersectWith(Line2d circle);
		public abstract IntersectionResult2d IntersectWith(Ray2d circle);
		public virtual IntersectionResult2d IntersectWith(IEnumerable<IIntersectionItem2d> polyCurve) => PolyCurveUtils.FindIntersect(polyCurve, Me);

		public IntersectionResult2d IntersectWith(IIntersectionItem2d target) => target switch
		{
			Segment2d seg => IntersectWith(seg),
			Line2d line => IntersectWith(line),
			Ray2d ray => IntersectWith(ray),
			Arc2d arc => IntersectWith(arc),
			Circle2d circle => IntersectWith(circle),
			IEnumerable<IIntersectionItem2d> polyCurve => IntersectWith(polyCurve),
			_ => throw new NotImplementedException()
		};
	}
}
