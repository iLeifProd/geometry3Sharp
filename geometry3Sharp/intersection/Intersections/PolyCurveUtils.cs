using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	public class PolyCurveUtils
	{
		public static IntersectionResult2d FindIntersect(IEnumerable<IIntersectionItem2d> me, IIntersectionItem2d target)
		{
			if (target is IEnumerable<IIntersectionItem2d> targetCollection)
			{
				return FindIntersect(me, targetCollection);
			}

			IntersectionResult2d totalResult = new();
			totalResult.ResultType = IntersectionProfile.Empty;

			bool isCollision = false;
			foreach (var curve in me)
			{
				var res = GetIntersection(curve, target);
				if (res.ResultType == IntersectionProfile.Empty)
				{
					continue;
				}

				if (res.ResultType == IntersectionProfile.Collision)
				{
					isCollision = true;
				}

				totalResult.ResultType = isCollision ? IntersectionProfile.Collision : IntersectionProfile.Point;
				totalResult.Points.AddRange(res.Points);
			}

			return totalResult;
		}

		public static IntersectionResult2d FindIntersect(IEnumerable<IIntersectionItem2d> me, IEnumerable<IIntersectionItem2d> targetCollection)
		{
			IntersectionResult2d totalResult = new();
			totalResult.ResultType = IntersectionProfile.Empty;

			bool isCollision = false;

			foreach (var target in targetCollection)
			{
				IntersectionResult2d semiResult = new();
				semiResult.ResultType = IntersectionProfile.Empty;

				foreach (var curve in me)
				{
					var res = GetIntersection(curve, target);
					if (res.ResultType == IntersectionProfile.Empty)
					{
						continue;
					}

					if (res.ResultType == IntersectionProfile.Collision)
					{
						isCollision = true;
					}

					semiResult.ResultType = isCollision ? IntersectionProfile.Collision : IntersectionProfile.Point;
					semiResult.Points.AddRange(res.Points);
				}

				totalResult.ResultType = isCollision ? IntersectionProfile.Collision : IntersectionProfile.Point;
				totalResult.Points.AddRange(semiResult.Points);
			}

			return totalResult;
		}

		private static IntersectionResult2d GetIntersection(IIntersectionItem2d one, IIntersectionItem2d two) => one switch
		{
			Segment2d seg => seg.Intersect(two),
			Line2d line => line.Intersect(two),
			Ray2d ray => ray.Intersect(two),
			Arc2d arc => arc.Intersect(two),
			Circle2d circle => circle.Intersect(two),
			IEnumerable<IIntersectionItem2d> polyCurve => ((IIntersectionItem2d)polyCurve).Intersect(two),
			_ => throw new NotImplementedException()
		};
	}
}
