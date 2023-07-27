using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	public class LineUtils2d
	{
		public static IntersectionResultParams2d FindIntersect(Vector2d P0, Vector2d D0, Vector2d P1, Vector2d D1,
			Limits limits0, Limits limits1, double tolerance)
		{
			IntersectionResultParams2d result = new();

			var lineToLine = FindIntersectParameters(P0, D0, P1, D1, tolerance);

			if (lineToLine.t != IntersectionProfile.Point)
			{
				result.ResultType = lineToLine.t;
				return result;
			}

			(double p1, double p2) = (lineToLine.t1.Value, lineToLine.t2.Value);
			if ((LimitsUtils.CheckLimitations(limits0, ref p1, tolerance) && LimitsUtils.CheckLimitations(limits1, ref p2, tolerance)) == false)
			{
				return result;
			}

			var point = p1 switch
			{
				0 => P0,
				1 => P0 + D0,
				_ => ComputePointByParameter(lineToLine.t1.Value, P0, D0), //P0 + s0 * D0
			};

			result.Add(point, p1, p2);
			result.ResultType = IntersectionProfile.Point;
			return result;
		}

		

		// The intersection of two lines is a solution to p1 + t1 * dir1 =
		// p2 + t2 * dir2. Rewrite this as t1*dir1 - t2*dir2 = p2 - p1 = Q. If
		// DotPerp(dir1, dir2)) = 0, the lines are parallel. Additionally, if
		// DotPerp(Q, dir2)) = 0, the lines are the same. If
		// DotPerp(dir1, dir2)) is not zero, then the lines intersect in a
		// single point where
		//   t1 = DotPerp(Q, dir2))/DotPerp(dir1, dir2))
		//   t2 = DotPerp(Q, dir1))/DotPerp(dir1, dir2))
		public static (IntersectionProfile t, double? t1, double? t2) FindIntersectParameters(Vector2d p1, Vector2d dir1, Vector2d p2, Vector2d dir2, double tolerance)
		{
			Vector2d Q = p2 - p1;
			double D0DotPerpD1 = dir1.DotPerp(dir2);

			if (Math.Abs(D0DotPerpD1) > tolerance)
			{
				// Lines intersect in a single point.
				double invD0DotPerpD1 = ((double)1) / D0DotPerpD1;
				double diffDotPerpD0 = Q.DotPerp(dir1);
				double diffDotPerpD1 = Q.DotPerp(dir2);
				double t1 = diffDotPerpD1 * invD0DotPerpD1;
				double t2 = diffDotPerpD0 * invD0DotPerpD1;
				return (IntersectionProfile.Point, t1, t2);
			}

			Q.Normalize();
			double diffNDotPerpD1 = Q.DotPerp(dir2);
			if (Math.Abs(diffNDotPerpD1) <= tolerance)
			{
				// Lines are colinear.
				return (IntersectionProfile.Collision, null, null);
			}

			return (IntersectionProfile.Empty, null, null);
		}

		//Compute point by parametric equation V = P0 + t1 * D0
		public static Vector2d ComputePointByParameter(double param, Vector2d origin, Vector2d dir)
		{
			return param == 0 ? origin : origin + dir * param;
		}
	}
}
