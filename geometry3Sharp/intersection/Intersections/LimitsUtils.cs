using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace g3.Intersections
{
	public enum Limits { Infinity, StartToEnd, StartToInfinity, EndToMinusInfinity }

	public static class LimitsUtils
	{
		public static bool CheckLimitations(Limits limits, ref double t, double tolerance) => limits switch
		{
			Limits.Infinity => true,
			Limits.StartToEnd => CheckStartToInfinityLimits(ref t, tolerance) && CheckEndToMinusInfinityLimits(ref t, tolerance),
			Limits.StartToInfinity => CheckStartToInfinityLimits(ref t, tolerance),
			Limits.EndToMinusInfinity => CheckEndToMinusInfinityLimits(ref t, tolerance),
			_ => false
		};

		public static bool CheckStartToInfinityLimits(ref double t, double tolerance)
		{
			if (t >= 0)
			{
				return true;
			}

			if (t >= -tolerance)
			{
				t = 0;
				return true;
			}
			return false;
		}

		public static bool CheckEndToMinusInfinityLimits(ref double t, double tolerance)
		{
			if (t <= 1)
			{
				return true;
			}

			if (t <= 1 + tolerance)
			{
				t = 1;
				return true;
			}

			return false;
		}
	}
}
