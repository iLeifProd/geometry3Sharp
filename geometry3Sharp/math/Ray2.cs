using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using g3.Intersections;

#if G3_USING_UNITY
using UnityEngine;
#endif

namespace g3
{
	public struct Ray2d : IIntersectionItem2d
	{
		public Vector2d Origin;
		public Vector2d Direction;

		public Ray2d(Vector2d origin, Vector2d direction, bool bIsNormalized = false)
		{
			this.Origin = origin;
			this.Direction = direction;
			if (bIsNormalized == false && Direction.IsNormalized == false)
				Direction.Normalize();
		}

		public IntersectionResult2d Intersect(IIntersectionItem2d target, double tolerance = MathUtil.ZeroTolerance)
		{
			RayIntersector intersector = new(this, tolerance);
			return intersector.IntersectWith(target);
		}

		// parameter is distance along ray
		public Vector2d PointAt(double d)
		{
			return Origin + d * Direction;
		}


		public double Project(Vector2d p)
		{
			return (p - Origin).Dot(Direction);
		}

		public double DistanceSquared(Vector2d p)
		{
			double t = (p - Origin).Dot(Direction);
			if (t < 0)
			{
				return Origin.DistanceSquared(p);
			}
			else
			{
				Vector2d proj = Origin + t * Direction;
				return (proj - p).LengthSquared;
			}
		}

		public Vector2d ClosestPoint(Vector2d p)
		{
			double t = (p - Origin).Dot(Direction);
			if (t < 0)
			{
				return Origin;
			}
			else
			{
				return Origin + t * Direction;
			}
		}

		public Line2d ToLine()
		{
			return new Line2d(Origin, Direction);
		}
	}
}
