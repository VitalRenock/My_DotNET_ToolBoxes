using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Mapper
{
	public abstract class IMapper<TOrigin, TTarget>
	{
		public abstract TOrigin ToOrigin(TTarget target);
		public abstract TTarget ToTarget(TOrigin origin);
	}
}
