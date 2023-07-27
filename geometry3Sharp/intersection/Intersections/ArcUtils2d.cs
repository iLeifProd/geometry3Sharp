using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	public class ArcUtils2d
	{
		public static IntersectionResult2d FindIntersect(Vector2d p, Vector2d dir, Arc2d arc, Limits lineLimits, double tolerance)
		{
			Circle2d circle = new(arc.Center, arc.Radius);
			IntersectionResultParams2d result = CircleUtils2d.FindIntersect(p, dir, circle, lineLimits, tolerance);

			if (result.ResultType != IntersectionProfile.Point)
			{
				return result;
			}

			result.ResultType = IntersectionProfile.Empty;
			foreach (var (point, param) in result.CurveParams.ToArray())
			{
				if (arc.Contains(point, tolerance) == false)
				{
					result.Points.Remove(point);
					continue;
				}
				result.ResultType = IntersectionProfile.Point;
			}

			return result;
		}

		public static IntersectionResult2d FindIntersect(Arc2d arc, Circle2d circle, double tolerance)
		{
			var result = CircleUtils2d.FindIntersect(circle, new Circle2d(arc.Center, arc.Radius), tolerance);

			if (result.ResultType == IntersectionProfile.Empty)
			{
				return result;
			}

			if (result.ResultType == IntersectionProfile.Point)
			{
				result.ResultType = IntersectionProfile.Empty;
				foreach (var iPoint in result.Points.ToArray())
				{
					if (arc.Contains(iPoint, tolerance) == false)
					{
						result.Points.Remove(iPoint);
						continue;
					}
					result.ResultType = IntersectionProfile.Point;
				}
				return result;
			}

			return result;
		}

		public static IntersectionResult2d FindIntersect(Arc2d arc1, Arc2d arc2, double tolerance)
		{
			var (circle1, circle2) = (new Circle2d(arc1.Center, arc1.Radius), new Circle2d(arc2.Center, arc2.Radius));
			var result = CircleUtils2d.FindIntersect(circle1, circle2, tolerance);

			if (result.ResultType == IntersectionProfile.Empty)
			{
				return result;
			}

			if (result.ResultType == IntersectionProfile.Point)
			{
				result.ResultType = IntersectionProfile.Empty;
				foreach (var iPoint in result.Points.ToArray())
				{
					if (arc1.Contains(iPoint, tolerance) == false && arc1.Contains(iPoint, tolerance) == false)
					{
						result.Points.Remove(iPoint);
						continue;
					}
					result.ResultType = IntersectionProfile.Point;
				}
				return result;
			}

			//TODO: Case when circles are cocircular ("coincidences")

			return result;
		}
	}
}
