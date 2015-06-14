using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
/*
	class AndValueProvider : AggregatedValueProvider<bool>
	{
		public AndValueProvider(IEnumerable<IValueProvider<bool>> operands)
			: base(operands)
		{
		}

		public AndValueProvider(params IValueProvider<bool>[] operands)
			: base(operands)
		{
		}

		protected override bool Calculate(IEnumerable<IValueProvider<bool>> source, IValueProvider<bool> changedProvider = null)
		{
			// compromizer value may greatly speedup aggregate calculation
			if (changedProvider != null)
			{
				// something is false - no need to check others
				if (!changedProvider.Value)
				{
#if DEBUG
					if (!source.Contains(changedProvider))
					{
						throw new Exception("Changed provider reported is no longer in source collection, cannot be used for optimization.");
					}
#endif
					return false;
				}
			}
			return source.All(x => x.Value);
		}
	}
*/
}