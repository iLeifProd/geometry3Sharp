using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using g3.Intersections;

namespace g3 {

	public class Arc2d : IParametricCurve2d
	{
		public Vector2d Center { get; set; }
		public double Radius;
		public double AngleStartDeg;
		public double AngleEndDeg;
		public bool IsReversed;     // use ccw orientation instead of cw

		public Vector2d StartDir => (SampleArcLength(1) - P0).Normalized;
		public Vector2d EndDir => (P1 - SampleArcLength(ArcLength - 1)).Normalized;

		public Arc2d(Vector2d center, double radius, double startDeg, double endDeg)
		{
			IsReversed = false;
			Center = center;
			Radius = radius;
			AngleStartDeg = startDeg;
			AngleEndDeg = endDeg;
			if ( AngleEndDeg < AngleStartDeg )
				AngleEndDeg += 360;

			// [TODO] handle full arcs, which should be circles?
		}


        /// <summary>
        /// Create Arc around center, **clockwise** from start to end points.
        /// Points must both be the same distance from center (ie on circle)
        /// </summary>
        public Arc2d(Vector2d vCenter, Vector2d vStart, Vector2d vEnd)
        {
            IsReversed = false;
            SetFromCenterAndPoints(vCenter, vStart, vEnd);
        }


        /// <summary>
        /// Initialize Arc around center, **clockwise** from start to end points.
        /// Points must both be the same distance from center (ie on circle)
        /// </summary>
        public void SetFromCenterAndPoints(Vector2d vCenter, Vector2d vStart, Vector2d vEnd)
        {
            Vector2d ds = vStart - vCenter;
            Vector2d de = vEnd - vCenter;
            //Debug.Assert(Math.Abs(ds.LengthSquared - de.LengthSquared) < MathUtil.ZeroTolerancef);
            AngleStartDeg = Math.Atan2(ds.y, ds.x) * MathUtil.Rad2Deg;
            AngleEndDeg = Math.Atan2(de.y, de.x) * MathUtil.Rad2Deg;
            if (AngleEndDeg < AngleStartDeg)
                AngleEndDeg += 360;
            Center = vCenter;
            Radius = ds.Length;
        }



		public Vector2d P0 {
			get { return SampleT(0.0); }
		}
		public Vector2d P1 {
			get { return SampleT(1.0); }
		}

        public double Curvature
        {
            get { return 1.0 / Radius; }
        }
        public double SignedCurvature
        {
            get { return (IsReversed) ? (-1.0 / Radius) : (1.0 / Radius); }
        }

		public bool IsClosed {
			get { return false; }
		}


		public double ParamLength {
			get { return 1.0f; }
		}


		// t in range[0,1] spans arc
		public Vector2d SampleT(double t) {
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
			double c = Math.Cos(theta), s = Math.Sin(theta);
			return new Vector2d(Center.x + Radius*c, Center.y + Radius*s);
		}


        public Vector2d TangentT(double t)
        {
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
            Vector2d tangent = new Vector2d(-Math.Sin(theta), Math.Cos(theta));
            if (IsReversed)
                tangent = -tangent;
            tangent.Normalize();
            return tangent;
        }

        public double? GetArcLength(Vector2d P)
        {
			if (Contains(P, MathUtil.Epsilon) == false)
			{
				return null;
			}

			Arc2d arcBeforeP = new(Center, P0, P);
            
            return arcBeforeP.ArcLength;
		}

		public (IParametricCurve2d left, IParametricCurve2d right)? Split(Vector2d p)
        {
			if (Contains(p) == false)
			{
                return null;
			}
            //TODO: Is need check P == P1, P0?
            Arc2d left = new(Center, P0, p);
            Arc2d right = new(Center, p, P1);

            return (left, right);
		}

		public bool HasArcLength { get {return true;} }

		public double ArcLength {
			get {
				return (AngleEndDeg-AngleStartDeg) * MathUtil.Deg2Rad * Radius;
			}
		}

		public Vector2d SampleArcLength(double a) {
            if (ArcLength < MathUtil.Epsilon)
                return (a < 0.5) ? SampleT(0) : SampleT(1);
			double t = a / ArcLength;
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
			double c = Math.Cos(theta), s = Math.Sin(theta);
			return new Vector2d(Center.x + Radius*c, Center.y + Radius*s);
		}

		public void Reverse() {
			IsReversed = ! IsReversed;
		}

        public IParametricCurve2d Clone() {
            return new Arc2d(this.Center, this.Radius, this.AngleStartDeg, this.AngleEndDeg) 
                { IsReversed = this.IsReversed };
        }


        public bool IsTransformable { get { return true; } }
        public void Transform(ITransform2 xform)
        {
            Vector2d vCenter = xform.TransformP(Center);
            Vector2d vStart = xform.TransformP((IsReversed) ? P1 : P0);
            Vector2d vEnd = xform.TransformP((IsReversed) ? P0 : P1);

            SetFromCenterAndPoints(vCenter, vStart, vEnd);
        }

        public AxisAlignedBox2d Bounds {
            get {
                // extrema of arc are P0, P1, and any axis-crossings that lie in arc span.
                // We can compute bounds of axis-crossings in normalized space and then scale/translate.
                int k = (int)(AngleStartDeg / 90.0);
                if (k * 90 < AngleStartDeg) 
                    k++;
                int stop_k = (int)(AngleEndDeg / 90);       
                if (stop_k * 90 > AngleEndDeg)
                    stop_k--;
                // [TODO] we should only ever need to check at most 4 here, right? then we have gone a circle...
                AxisAlignedBox2d bounds = AxisAlignedBox2d.Empty;
                while (k <= stop_k) {
                    int i = k++ % 4;
                    bounds.Contain(bounds_dirs[i]);
                }
                bounds.Scale(Radius); bounds.Translate(Center);
                bounds.Contain(P0); bounds.Contain(P1);
                return bounds;
            }
        }



		private static readonly Vector2d[] bounds_dirs = new Vector2d[4] {
            Vector2d.AxisX, Vector2d.AxisY, -Vector2d.AxisX, -Vector2d.AxisY };

		public IntersectionResult2d Intersect(IIntersectionItem2d target, double tolerance = MathUtil.ZeroTolerance)
		{
			ArcIntersector2d intersector = new(this, tolerance);
			return intersector.IntersectWith(target);
		}

		// This function assumes P is on the circle containing the arc (with
		// possibly a small amount of floating-point rounding error). NOTE:
		// I have kept this code so that clients within the GTE library are
		// not broken if instead I had removed this function. It will be
		// deprecated for GTL.
		public bool Contains(Vector2d P)
        {
			Vector2d diffPE0 = P - P0;
		    Vector2d diffE1E0 = P1 - P0;
		    double dotPerp = diffPE0.DotPerp(diffE1E0);
            return dotPerp >= 0;
        }

	    // Test whether P is on the arc.
	    // 
	    // Formulated for real arithmetic, |P-C| - r = 0 is necessary for P to
	    // be on the circle of the arc. If P is on the circle, then P is on
	    // the arc from E0 to E1 when it is on the side of the line containing
	    // E0 with normal Perp(E1-E0) where Perp(u,v) = (v,-u). This test
	    // works for any angle between E0-C and E1-C, even if the angle is
	    // larger or equal to pi radians.
	    //
	    // Formulated for floating-point or rational types, rounding errors
	    // cause |P-C| - r rarely to be 0 when P is on (or numerically near)
	    // the circle. To allow for this, choose a small and nonnegative
	    // tolerance epsilon. The test concludes that P is on the circle when
	    // ||P-C| - r| <= epsilon;otherwise, P is not on the circle. If P is
	    // on the circle (in the epsilon-tolerance sense), the side-of-line
	    // test of the previous/ paragraph is applied.
	    public bool Contains(Vector2d P, double epsilon)
        {
            // If epsilon is negative, the tolerance behaves as if a value of
            // zero was passed for epsilon.

			double length = P.Distance(Center);
			if (Math.Abs(length - Radius) <= epsilon)
            {
                Vector2d diffPE0 = P - P0;
		        Vector2d diffE1E0 = P1 - P0;
		        double dotPerp = diffPE0.DotPerp(diffE1E0);
                return dotPerp >= 0;
            }
            else
            {
                return false;
            }
        }

		public Line2d Perpendicular(Vector2d toP)
        {
			var res = Circle2d.FindPerpendicular(toP, Center, Radius);

			if (res.isPtOnCenter == true || res.line == null)
			{
				return Line2d.FromPoints(SampleT(0.5), toP);
			}

			return res.line.Value;
		}


		public double Distance(Vector2d point)
        {
            Vector2d PmC = point - Center;
            double lengthPmC = PmC.Length;
            if (lengthPmC > MathUtil.Epsilon) {
                Vector2d dv = PmC / lengthPmC;
				double theta = Math.Atan2(dv.y, dv.x) * MathUtil.Rad2Deg;
				if ( ! (theta >= AngleStartDeg && theta <= AngleEndDeg) ) {
					double ctheta = MathUtil.ClampAngleDeg(theta, AngleStartDeg, AngleEndDeg);
                    double radians = ctheta * MathUtil.Deg2Rad;
					double c = Math.Cos(radians), s = Math.Sin(radians);
                    Vector2d pos = new Vector2d(Center.x + Radius * c, Center.y + Radius * s);
					return pos.Distance(point);
                } else {
					return Math.Abs(lengthPmC - Radius);
                }
            } else {
                return Radius;
            }
		}

		public bool IsClockwise()
		{
			bool result = false;

			if ((P0 - Center).DotPerp(P1 - Center) < 0)
			{
				result = !result;
			}

			return result;

			//return IsReversed ? !result : result;
		}
		public Vector2d NearestPoint(Vector2d point)
        {
            Vector2d PmC = point - Center;
            double lengthPmC = PmC.Length;
            if (lengthPmC > MathUtil.Epsilon) {
                Vector2d dv = PmC / lengthPmC;
                double theta = Math.Atan2(dv.y, dv.x);
                theta *= MathUtil.Rad2Deg;
                theta = MathUtil.ClampAngleDeg(theta, AngleStartDeg, AngleEndDeg);
                theta = MathUtil.Deg2Rad * theta;
                double c = Math.Cos(theta), s = Math.Sin(theta);
                return new Vector2d(Center.x + Radius * c, Center.y + Radius * s);
            } else 
                return SampleT(0.5);        // all points equidistant
        }

        public double? FindAngleDeg(Vector2d point, double tolerance)
        {
            //TODO: Not tested
            Vector2d pDiff = point - Center;

			if (Math.Abs(Radius * Radius - pDiff.LengthSquared) <= tolerance)
			{
				return null;
			}

			double pAngle = Math.Atan2(pDiff.y, pDiff.x);
            double pAngleDeg = pAngle * MathUtil.Rad2Deg;

            if (pAngleDeg < AngleStartDeg)
            {
                if (pAngle <= AngleStartDeg - tolerance)
				{
                    return null;
                }
                pAngle = AngleStartDeg;
            }
			else if (pAngleDeg > AngleEndDeg)
			{
				if (pAngle >= AngleEndDeg + tolerance)
				{
					return null;
				}
				pAngle = AngleEndDeg;
			}

            return pAngle;
			//if (AngleStartDeg < pAngleDeg - tolerance && AngleEndDeg > pAngleDeg + tolerance)
            //{
            //    return null;
            //}
        }
	}
}
