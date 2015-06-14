using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	class SumAggregatedValueProviderFromVP : AggregatedValueProviderFromVP<int, int>
	{
		public SumAggregatedValueProviderFromVP(IEnumerable<IValueProvider<int>> operands)
			: base(operands)
		{
		}

		protected override int Calculate(IEnumerable<IValueProvider<int>> source)
		{
			return source.Select(x => x.Value).Sum();
		}
	}
}