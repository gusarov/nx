using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	static class SelectObservableEnumerable
	{
		public static SelectObservableEnumerable<TSource, TResult> Create<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector, SelectorBehaviour selectorBehaviour)
		{
			return new SelectObservableEnumerable<TSource, TResult>(source, selector, selectorBehaviour);

			switch (selectorBehaviour)
			{
				case SelectorBehaviour.None:
					break;
				case SelectorBehaviour.Cache:
					break;
				case SelectorBehaviour.Listen:
					break;
				default:
					throw new ArgumentOutOfRangeException("selectorBehaviour");
			}
		}
	}
}