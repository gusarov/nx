using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NX.Internal;

namespace NX
{
	public static class ObservableEnumerableExt
	{
		public static IValueProvider<int> SumO(this IEnumerable<int> source)
		{
			return WeakKeyCache<object, SumAggregatedValueProvider>.Global[source, () =>
				new SumAggregatedValueProvider(source)];
		}

		public static IValueProvider<int> SumO(this IEnumerable<IValueProvider<int>> source)
		{
			return WeakKeyCache<object, SumAggregatedValueProviderFromVP>.Global[source, () =>
				new SumAggregatedValueProviderFromVP(source)];
		}
/*
		public static IValueProvider<int> SumO<T>(this IEnumerable<T> source, Func<T, IValueProvider<int>> valueProvider)
		{
			return source.SumO();
		}
		public static IValueProvider<int> SumO<T>(this IEnumerable<T> source, Func<T, int> valueProvider)
		{
			return source.SumO();
		}
*/

		public static IEnumerable<TR> SelectO<TS, TR>(this IEnumerable<TS> source, Func<TS, TR> selector, SelectorBehaviour selectorBehaviour = SelectorBehaviour.None)
		{
			return new SelectObservableEnumerable<TS, TR>(source, selector, selectorBehaviour);
		}

		public static IEnumerable<T> WhereO<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return new WhereObservableEnumerable<T>(source, predicate);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public enum SelectorBehaviour
	{
		/// <summary>
		/// Do not subscribe and do not cache.
		/// </summary>
		None,
		/// <summary>
		/// Cache selections. If we caching - there is no reason to subscribe and react to properties. If an element removed and then returned back to collection - it will likely reuse previous selector result.
		/// </summary> 
		/// <example>
		/// <code>
		/// models.SelectO(x => new ViewModel(x), SelectorBehaviour.Cache).
		/// </code>
		/// </example>
		Cache,
		/// <summary>
		/// Subscribe to collection item's INotifyPropertyChanged. If we subscribed - we should not cache.
		/// </summary>
		/// <example>
		/// <code>
		/// models.SelectO(x => x.Pro, SelectorBehaviour.Listen).
		/// </code>
		/// </example>
		Listen,
	}
}
