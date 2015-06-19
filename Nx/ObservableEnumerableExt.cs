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
		/// <summary>
		/// Sum Observable
		/// </summary>
		public static IValueProvider<int> SumO(this IEnumerable<int> source)
		{
			return WeakKeyCache<object, SumAggregatedValueProvider>.Global[source, () =>
				new SumAggregatedValueProvider(source)];
		}

		/// <summary>
		/// Sum Observable
		/// </summary>
		public static IValueProvider<int> SumO(this IEnumerable<IValueProvider<int>> source)
		{
			return WeakKeyCache<object, SumAggregatedValueProviderFromVP>.Global[source, () =>
				new SumAggregatedValueProviderFromVP(source)];
		}

		/// <summary>
		/// Sum Observable
		/// </summary>
		public static IValueProvider<int> SumO<T>(this IEnumerable<T> source, Func<T, IValueProvider<int>> valueProvider)
		{
			return source.SelectO(valueProvider).SumO();
		}

		/// <summary>
		/// Sum Observable
		/// </summary>
		public static IValueProvider<int> SumO<T>(this IEnumerable<T> source, Func<T, int> valueProvider)
		{
			return source.SelectO(valueProvider).SumO();
		}

		/// <summary>
		/// Select Observable
		/// </summary>
		public static IEnumerable<TR> SelectO<TS, TR>(this IEnumerable<TS> source, Func<TS, TR> selector, SelectorBehaviour selectorBehaviour = SelectorBehaviour.None)
		{
			return new SelectObservableEnumerable<TS, TR>(source, selector, selectorBehaviour);
		}

		/// <summary>
		/// Where Observable
		/// </summary>
		public static IEnumerable<T> WhereO<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return new WhereObservableEnumerable<T>(source, predicate);
		}
	}
}
