using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace g3.Intersections
{
	public class CircleUtils2d
	{
		public static IntersectionResultParams2d FindIntersect(Vector2d p, Vector2d dir, Circle2d circle,
			Limits lineLimits, double tolerance)
		{
			IntersectionResultParams2d result = new() { ResultType = IntersectionProfile.Empty };

			var intersections = FindIntersectParameters(p, dir, circle, tolerance);

			if (intersections.t != IntersectionProfile.Point)
			{
				result.ResultType = intersections.t;
				return result;
			}


			foreach (var intersection in intersections.lineParams)
			{
				double param = intersection;

				if (LimitsUtils.CheckLimitations(lineLimits, ref param, tolerance) == false)
				{
					continue;
				}

				Vector2d intersectionPoint = LineUtils2d.ComputePointByParameter(param, p, dir);
				result.Add(intersectionPoint, param);
			}

			return result;
		}

		// The two circles are |X-C0| = R0 and |X-C1| = R1.  Define
		// U = C1 - C0 and V = Perp(U) where Perp(x,y) = (y,-x).  Note
		// that Dot(U,V) = 0 and |V|^2 = |U|^2.  The intersection points X
		// can be written in the form X = C0+s*U+t*V and
		// X = C1+(s-1)*U+t*V.  Squaring the circle equations and
		// substituting these formulas into them yields
		//   R0^2 = (s^2 + t^2)*|U|^2
		//   R1^2 = ((s-1)^2 + t^2)*|U|^2.
		// Subtracting and solving for s yields
		//   s = ((R0^2-R1^2)/|U|^2 + 1)/2
		// Then replace in the first equation and solve for t^2
		//   t^2 = (R0^2/|U|^2) - s^2.
		// In order for there to be solutions, the right-hand side must be
		// nonnegative.  Some algebra leads to the condition for existence
		// of solutions,
		//   (|U|^2 - (R0+R1)^2)*(|U|^2 - (R0-R1)^2) <= 0.
		// This reduces to
		//   |R0-R1| <= |U| <= |R0+R1|.
		// If |U| = |R0-R1|, then the circles are side-by-side and just
		// tangent.  If |U| = |R0+R1|, then the circles are nested and
		// just tangent.  If |R0-R1| < |U| < |R0+R1|, then the two circles
		// to intersect in two points.

		public static IntersectionResult2d FindIntersect(Circle2d circle1, Circle2d circle2, double tolerance)
		{
			IntersectionResultParams2d result = new();

			Vector2d U = circle2.Center - circle1.Center;
			double USqrLen = U.Dot(U);
			double R0 = circle1.Radius, R1 = circle2.Radius;
			double R0mR1 = R0 - R1;

			if (USqrLen.EpsilonEqual(tolerance) && R0mR1.EpsilonEqual(tolerance))
			{
				// Circles are the same.
				result.ResultType = IntersectionProfile.Collision;
				return result;
			}

			double R0mR1Sqr = R0mR1 * R0mR1;
			if (USqrLen < R0mR1Sqr)
			{
				// The circles do not intersect.
				return result;
			}

			double R0pR1 = R0 + R1;
			double R0pR1Sqr = R0pR1 * R0pR1;
			if (USqrLen > R0pR1Sqr)
			{
				// The circles do not intersect.
				return result;
			}

			result.ResultType = IntersectionProfile.Point;
			if (USqrLen >= R0pR1Sqr - tolerance)
			{
				// |U| = |R0+R1|, circles are tangent.
				result.Add(circle1.Center + (R0 / R0pR1) * U);
				return result;
			}

			if (R0mR1Sqr >= USqrLen - tolerance)
			{
				// |U| = |R0-R1|, circles are tangent.
				result.Add(circle1.Center + (R0 / R0mR1) * U);
				return result;
			}
			
			double invUSqrLen = 1 / USqrLen;
			double s = 0.5 * ((R0 * R0 - R1 * R1) * invUSqrLen + 1);
			Vector2d tmp = circle1.Center + s * U;

			// In theory, discr is nonnegative.  However, numerical round-off
			// errors can make it slightly negative.  Clamp it to zero.
			double discr = R0 * R0 * invUSqrLen - s * s;
			if (discr < 0)
			{
				discr = 0;
			}
			double t = Math.Sqrt(discr);
			Vector2d V = new(U[1], -U[0]);
			result.Add(tmp - t * V);
			if (t > 0)
			{
				result.Add(tmp + t * V);
			}

			return result;
		}

		// Intersection of a the line P+t*D and the circle |X-C| = R.
		// The line direction is unit length. The t-value is a
		// real-valued root to the quadratic equation
		//   0 = |t*D+P-C|^2 - R^2
		//     = t^2 + 2*Dot(D,P-C)*t + |P-C|^2-R^2
		//     = t^2 + 2*a1*t + a0
		// If there are two distinct roots, the order is t0 < t1.
		public static (IntersectionProfile t, List<double> lineParams) FindIntersectParameters(Vector2d p, Vector2d dir, Circle2d circle, double tolerance)
		{
			(IntersectionProfile t, List<double> lineParams) result = new(IntersectionProfile.Empty, new());

			Vector2d diff = p - circle.Center;
			double a0 = diff.Dot(diff) - circle.Radius * circle.Radius;
			double a1 = dir.Dot(diff);
			double discr = a1 * a1 - a0;

			if (discr == 0)
			{
				return result;
			}

			result.t = IntersectionProfile.Point;
			if (discr > 0)
			{
				double root = Math.Sqrt(discr);
				result.lineParams.Add(-a1 - root);
				result.lineParams.Add(-a1 + root);
				return result;
			}

			//discr < 0
			result.lineParams.Add(-a1);
			return result;
		}
	}
}

