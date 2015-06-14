using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	internal abstract class AggregatedValueProviderFromVP<TR, TS> : AggregatedValueProvider<TR, IValueProvider<TS>>
	{
		protected AggregatedValueProviderFromVP(IEnumerable<IValueProvider<TS>> operands)
			: base(operands)
		{
		}
	}
}