using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3.Intersections
{
	public interface IIntersectionItem2d
	{
		IntersectionResult2d Intersect(IIntersectionItem2d target, double tolerance = MathUtil.ZeroTolerance)
		{
			throw new NotImplementedException();
		}
	}
}
