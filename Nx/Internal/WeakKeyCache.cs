using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	class WeakKeyCache<TKey, TValue> : SelfMaintainable
	{
		private static WeakKeyCache<TKey, TValue> _global;

		public static WeakKeyCache<TKey, TValue> Global
		{
			get { return _global ?? (_global = new WeakKeyCache<TKey, TValue>()); }
		}

		readonly Dictionary<EquatableWeakReference, TValue> _dic = new Dictionary<EquatableWeakReference, TValue>(WeakKeyComparer.Comparer);

		public int Count
		{
			get { return _dic.Count; }
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> Entries
		{
			get
			{
				lock (_sync)
				{
					return _dic.Select(x => new KeyValuePair<TKey, TValue>((TKey)x.Key.Target, x.Value)).Where(x => !Equals(x.Key, default(TKey))).ToArray();
				}
			}
		}

		void SetWeak(TKey weakKey, TValue value)
		{
			lock (_sync)
			{
				ScavengeKeys();
				_dic[new EquatableWeakReference(weakKey)] = value;
			}
		}

		readonly object _sync = new object();

		TValue TryGet(TKey weakKey)
		{
			lock (_sync)
			{
				TValue value;
				// todo extra object generated every time
				_dic.TryGetValue(new EquatableWeakReference(weakKey), out value);
				return value;
			}
		}

		public TValue this[TKey key]
		{
			get { return TryGet(key); }
			set { SetWeak(key, value); }
		}

		/// <summary>
		/// Get or create
		/// </summary>
		public TValue this[TKey key, Func<TValue> factory]
		{
			get
			{
				var cached = this[key];
				if (IsDefault(cached))
				{
					this[key] = cached = factory();
				}
				return cached;
			}
		}

		/// <summary>
		/// Get or create
		/// </summary>
		public TValue this[TKey key, Func<TKey, TValue> factory]
		{
			get
			{
				var cached = this[key];
				if (IsDefault(cached))
				{
					this[key] = cached = factory(key);
				}
				return cached;
			}
		}

		bool IsDefault(TValue value)
		{
			if (typeof(TValue).IsValueType)
			{
				return default(TValue).Equals(value);
			}
			else
			{
				return ReferenceEquals(null, value);
			}
		}

		long _lastGlobalMem;
		int _lastHashCount;
		byte _seed;

		private void ScavengeKeys()
		{
			if (IsScheduled)
			{
				// do not perform extra work, if timer is activated
				return;
			}
			if (unchecked(++_seed == 0))
			{
				var count = _dic.Count;
				if (count != 0)
				{
					if (_lastHashCount == 0)
					{
						_lastHashCount = count;
					}
					else
					{
						var totalMemory = GC.GetTotalMemory(false);
						if (_lastGlobalMem == 0L)
						{
							_lastGlobalMem = totalMemory;
						}
						else
						{
							if ((totalMemory < _lastGlobalMem) && (count >= _lastHashCount))
							{
								PerformScavage();
							}
							_lastGlobalMem = totalMemory;
							_lastHashCount = count;
						}
					}
				}
			}
		}

		void PerformScavage()
		{
			lock (_sync)
			{
				foreach (var deadKey in _dic.Keys.Where(x => !x.IsAlive).ToArray())
				{
					_dic.Remove(deadKey);
				}
			}
		}

		protected override void Maintain()
		{
			PerformScavage();
		}
	}
}