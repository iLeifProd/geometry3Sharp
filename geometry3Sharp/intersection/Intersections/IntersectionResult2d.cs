using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	public enum IntersectionProfile
	{
		Empty, Point, Collision
	}

	public class IntersectionResult2d
	{
		public IntersectionProfile ResultType { get; set; } = IntersectionProfile.Empty;
		public List<Vector2d> Points { get; set; } = new();
		public virtual Vector2d? Last => Points.FirstOrDefault();
		public virtual Vector2d? First => Points.FirstOrDefault();

		public static IntersectionResult2d Empty => new();
		
		public virtual void Add(Vector2d v)
		{
			Points.Add(v);
		}
	}

	public class IntersectionResultParams2d : IntersectionResult2d
	{
		public Dictionary<Vector2d, (double? one, double? two)> CurveParams { get; set; } = new();

		public (Vector2d v, (double? one, double? two) param)? LastParams => CurveParams.Any() ?
			(CurveParams.Last().Key, CurveParams.Last().Value) : null;
		public (Vector2d v, (double? one, double? two) param)? FirstParams => CurveParams.Any() ?
			(CurveParams.First().Key, CurveParams.First().Value) : null;

		public void Add(Vector2d v, double? oneParam = null, double? twoParam = null)
		{
			base.Add(v);
			CurveParams.Add(v, (oneParam, twoParam));
		}
	}
}
