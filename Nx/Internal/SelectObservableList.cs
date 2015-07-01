using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	internal class SelectObservableList<TS, TR> : NxObservableReadonlyList<TR>
	{
		private readonly IList<TS> _sourceListt;
		private readonly Func<TS, TR> _selector;
		private readonly SelectorBehaviour _selectorBehaviour;

		private readonly WeakKeyCache<TS, TR> _cache = new WeakKeyCache<TS, TR>();

		public SelectObservableList(IList<TS> source, Func<TS, TR> selector, SelectorBehaviour selectorBehaviour)
		{
			_sourceListt = source;
			_selector = selector;
			_selectorBehaviour = selectorBehaviour;
			if (_sourceListt == null)
			{
				throw new InvalidOperationException();
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public override IEnumerator<TR> GetEnumerator()
		{
			int id = 0;
			while (true)
			{
				var r = Get(id++);
				if (id > 1000000)
				{
					throw new Exception("Too long iteration");
				}
				yield return r;
			}
		}

		protected override void CopyToCore(Array array, int index)
		{
			for (int i = 0, c = Count; i < c; i++)
			{
				array.SetValue(Get(i), index + i);
			}
		}

		protected override void CopyToCore(TR[] array, int index)
		{
			for (int i = 0, c = Count; i < c; i++)
			{
				array[index + i] = Get(i);
			}
		}

		protected override TR Get(int id)
		{
			var src = _sourceListt[id++];
			switch (_selectorBehaviour)
			{
				case SelectorBehaviour.None:
				case SelectorBehaviour.Listen:
					return _selector(src);
				case SelectorBehaviour.Cache:
					return _cache[src, x => _selector(x)];
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public override int Count
		{
			get { return _sourceListt.Count; }
		}
	}
}