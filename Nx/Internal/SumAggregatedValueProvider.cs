using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	class SumAggregatedValueProvider : AggregatedValueProvider<int, int>
	{
		public SumAggregatedValueProvider(IEnumerable<int> operands) : base(operands)
		{
		}

		protected override int Calculate(IEnumerable<int> source)
		{
			return source.Sum();
		}
	}
}