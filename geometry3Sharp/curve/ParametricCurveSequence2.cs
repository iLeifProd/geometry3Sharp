using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using g3.Intersections;

namespace g3 
{
	public class ParametricCurveSequence2 : IParametricCurve2d, IMultiCurve2d
	{

		List<IParametricCurve2d> curves;
		bool closed;

		public Vector2d P0 => SampleT(0);
		public Vector2d P1 => SampleT(ParamLength);
		public Vector2d Center => SampleT(ParamLength / 2);

		public ParametricCurveSequence2() {
			curves = new List<IParametricCurve2d>();
		}

		public ParametricCurveSequence2(IEnumerable<IParametricCurve2d> curvesIn, bool isClosed = false) { 
			curves = new List<IParametricCurve2d>(curvesIn);
            closed = isClosed;
		}

		public int Count {
			get { return curves.Count; }
		}

		public ReadOnlyCollection<IParametricCurve2d> Curves {
			get { return curves.AsReadOnly(); }
		}

		public bool IsClosed { 
			get { return closed; }
			set { closed = value; }
		}


		public void Append(IParametricCurve2d c) {
			// sanity checking??
			curves.Add(c);
		}

		public void Prepend(IParametricCurve2d c) {
			curves.Insert(0, c);
		}


		public double ParamLength {
			get { 
				double sum = 0;
				foreach ( var c in Curves )
					sum += c.ParamLength;
				return sum;
			}
		}
		public bool Contains(Vector2d P, double epsilon) => Curves.Any(c => c.Contains(P, epsilon) == true);

		public Vector2d SampleT(double t) {
			double sum = 0;
			for ( int i = 0; i < Curves.Count; ++i ) {
				double l = curves[i].ParamLength;
				if (t <= sum+l) {
					double ct = (t - sum);
					return curves[i].SampleT(ct);
				}
				sum += l;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleT: argument out of range");
		}

		public Vector2d TangentT(double t) {
			double sum = 0;
			for ( int i = 0; i < Curves.Count; ++i ) {
				double l = curves[i].ParamLength;
				if (t <= sum+l) {
					double ct = (t - sum);
					return curves[i].TangentT(ct);
				}
				sum += l;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleT: argument out of range");
		}



		public bool HasArcLength { get { 
				foreach ( var c in Curves )
					if ( c.HasArcLength == false )
						return false;
				return true;
			} 
		}

		public double ArcLength {
			get {
				double sum = 0;
				foreach ( var c in Curves )
					sum += c.ArcLength;
				return sum;
			}
		}

		public Vector2d SampleArcLength(double a) {
			double sum = 0;
			for ( int i = 0; i < Curves.Count; ++i ) {
				double l = curves[i].ArcLength;
				if (a <= sum+l) {
					double ca = (a - sum);
					return curves[i].SampleArcLength(ca);
				}
				sum += l;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleArcLength: argument out of range");
		}

		public void Reverse() {
			foreach ( var c in curves )
				c.Reverse();
			curves.Reverse();
		}

        public IParametricCurve2d Clone() {
            ParametricCurveSequence2 s2 = new ParametricCurveSequence2();
            s2.closed = this.closed;
            s2.curves = new List<IParametricCurve2d>();
            foreach (var c in this.curves)
                s2.curves.Add(c.Clone());
            return s2;
        }


        public bool IsTransformable { get { return true; } }
        public void Transform(ITransform2 xform)
        {
            foreach (var c in this.curves)
                c.Transform(xform);
        }

		public IntersectionResult2d Intersect(IIntersectionItem2d target, double tolerance = MathUtil.ZeroTolerance)
		{
			PolyCurveIntersector2d intersector = new(this, tolerance);
			return intersector.IntersectWith(target);
		}

		public IEnumerator<IParametricCurve2d> GetEnumerator() => curves.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}
}
