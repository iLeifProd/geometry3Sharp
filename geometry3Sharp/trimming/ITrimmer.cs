using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace g3.Trimming
{
	public interface ITrimmer
	{
		Vector2d FromEnd();
		Vector2d FromStart();
		Vector2d FromBoth();
	}
}
