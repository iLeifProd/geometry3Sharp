using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3
{
	public class CurvePair
	{
		public IParametricCurve2d One { get; set; }
		public IParametricCurve2d? Two { get; set; }

		public CurvePair(IParametricCurve2d one, IParametricCurve2d? two = null)
		{
			One = one;
			Two = two;
		}

		public void Swap()
		{
			if (Two == null)
			{
				throw new NoNullAllowedException("Trying to swap null segment");
			}

			(One, Two) = (Two, One);
		}
	}
}
