using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using g3.Intersections;
using g3.Trimming;

namespace g3
{

	public interface IParametricCurve3d
	{
		bool IsClosed {get;}

		// can call SampleT in range [0,ParamLength]
		double ParamLength {get;}
		Vector3d SampleT(double t);
		Vector3d TangentT(double t);        // returns normalized vector

		bool HasArcLength {get;}
		double ArcLength {get;}
		Vector3d SampleArcLength(double a);

		void Reverse();

		IParametricCurve3d Clone();		
	}




    public interface ISampledCurve3d
    {
        int VertexCount { get; }
        int SegmentCount { get; }
        bool Closed { get; }

        Vector3d GetVertex(int i);
        Segment3d GetSegment(int i);

        IEnumerable<Vector3d> Vertices { get; }
    }





	public interface IParametricCurve2d : IIntersectionItem2d
	{
		Vector2d P0 { get; }
		Vector2d P1 { get; }
		Vector2d Center { get; }
		Vector2d StartDir => (SampleT(0.05) - P0).Normalized;
		Vector2d EndDir => (SampleT(0.95) - P1).Normalized;

		bool IsClosed {get;}

		// can call SampleT in range [0,ParamLength]
		double ParamLength {get;}
		Vector2d SampleT(double t);
        Vector2d TangentT(double t);        // returns normalized vector
		Line2d Perpendicular(Vector2d P) => throw new NotImplementedException();
		double? GetArcLength(Vector2d P) => throw new NotImplementedException();
		CurvePair? Split(Vector2d p) => throw new NotImplementedException();  
		bool Contains(Vector2d P, double epsilon = MathUtil.ZeroTolerance);
		Vector2d NearestPoint(Vector2d point) => throw new NotImplementedException();
		double DistanceTo(Vector2d point) => throw new NotImplementedException();
		double DistanceSquaredTo(Vector2d point) => throw new NotImplementedException();

		bool HasArcLength {get;}
		double ArcLength {get;}
		Vector2d SampleArcLength(double a);

		void Reverse();

        bool IsTransformable { get; }
        void Transform(ITransform2 xform);

		//ITrimmer Trim { get; } 


		IParametricCurve2d Clone();
	}


    public interface IMultiCurve2d : IEnumerable<IParametricCurve2d>, IParametricCurve2d
    {
        ReadOnlyCollection<IParametricCurve2d> Curves { get; }
    }

}
