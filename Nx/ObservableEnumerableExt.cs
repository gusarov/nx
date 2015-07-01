using System;
using System.Collections;
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
		/// Select Observable wich cached selector. Useful for factories of new object.
		/// </summary>
		/// <example>
		/// <code>
		/// models.SelectTo(x => new ViewModel(x))
		/// </code>
		/// </example>
		public static IEnumerable<TR> SelectTo<TS, TR>(this IEnumerable<TS> source, Func<TS, TR> selector)
		{
			return SelectO<TS, TR>(source, selector, SelectorBehaviour.Cache);
		}

		/// <summary>
		/// Select Observable subscribing to each item. Useful for child property accessors.
		/// </summary>
		/// <example>
		/// <code>
		/// models.SelectOFrom(x => x.Pro)
		/// </code>
		/// </example>

		public static IEnumerable<TR> SelectFrom<TS, TR>(this IEnumerable<TS> source, Func<TS, TR> selector)
		{
			return SelectO<TS, TR>(source, selector, SelectorBehaviour.Listen);
		}

		/// <summary>
		/// Select Observable
		/// </summary>
		public static IEnumerable<TR> SelectO<TS, TR>(this IEnumerable<TS> source, Func<TS, TR> selector, SelectorBehaviour selectorBehaviour = SelectorBehaviour.None)
		{
			var listt = source as IList<TS>;
			if (listt != null)
			{
				return new SelectObservableList<TS, TR>(listt, selector, selectorBehaviour);
			}
			return new SelectObservableEnumerable<TS, TR>(source, selector, selectorBehaviour);
		}

		/// <summary>
		/// Where Observable
		/// </summary>
		public static IEnumerable<T> WhereO<T>(this IEnumerable<T> source, Func<T, bool> predicate, object additionalEventSource = null)
		{
			/*
			var listt = source as IList<T>;
			if (listt != null)
			{
				return new WhereObservableEnumerable<T>(listt, predicate, additionalEventSource);
			}
			*/
			return new WhereObservableEnumerable<T>(source, predicate, additionalEventSource);
		}

		public static IEnumerable<T> TakeO<T>(this IEnumerable<T> source, int max)
		{
			var listt = source as IList<T>;
			if (listt != null)
			{
				return new TakeObservableList<T>(listt, max);
			}
			return new TakeObservableEnumerable<T>(source, max);
		}
	}
}
