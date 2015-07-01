using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Nx.Sample
{
	sealed class ExtremelyLargeCollection<T> : IEnumerable<T>, IList, IList<T>, INotifyPropertyChanged, INotifyCollectionChanged where T : class, new()
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new LargeEtor<T>(this);
		}

		private readonly Dictionary<int, T> _cachedIdToItem = new Dictionary<int, T>();

		private Dictionary<object, int> _cachedItemToId;

		private Dictionary<object, int> CachedItemToId
		{
			get
			{
				if (_cachedItemToId == null)
				{
					Trace.TraceWarning("ExtremelyLargeCollection: Backward index building");
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}

					// build backward index
					if (_cachedItemToId == null)
					{
						_cachedItemToId = _cachedIdToItem.ToDictionary(x => (object)x.Value, x => x.Key);
					}

				}
				return _cachedItemToId;
			}
		}

		internal T Get(int id)
		{
			T item;
			if (!_cachedIdToItem.TryGetValue(id, out item))
			{
				_cachedIdToItem[id] = item = (T)Activator.CreateInstance(typeof(T), id); // new T();
				OnPropertyChnaged("CachedCount");
				if (_cachedItemToId != null)
				{
					_cachedItemToId[item] = id;
				}
			}
			return item;
		}

		int ICollection.Count
		{
			get { return int.MaxValue; }
		}

		object IList.this[int index]
		{
			get { return Get(index); }
			set
			{
				throw new NotSupportedException("Readonly!");
			}
		}

		int IList.IndexOf(object value)
		{
			return IndexOfCore(value);
		}
		int IndexOfCore(object value)
		{
			if (Get(0) == value) // shortcut for WPF virtualization
			{
				return 0;
			}

			int id;
			if (CachedItemToId.TryGetValue(value, out id))
			{
				return id;
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Readonly!");
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Readonly!");
		}

		bool IList.Contains(object value)
		{
			return CachedItemToId.ContainsKey(value);
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Readonly!");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Readonly!");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Readonly!");
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException("Cannot allocate int.MaxValue array");
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChnaged(string name)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			var handler = CollectionChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		int IList<T>.IndexOf(T item)
		{
			return IndexOfCore(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		T IList<T>.this[int index]
		{
			get { return Get(index); }
			set
			{
				throw new NotImplementedException();
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotImplementedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<T>.Contains(T item)
		{
			throw new NotImplementedException();
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		int ICollection<T>.Count
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}
	}

	class LargeEtor<T> : IEnumerator<T> where T : class, new()
	{
		private readonly ExtremelyLargeCollection<T> _largeCollection;
		private int _id = -1;

		public LargeEtor(ExtremelyLargeCollection<T> largeCollection)
		{
			_largeCollection = largeCollection;
		}

		private T _current;
		T GetCurrent()
		{
			if (_id < 0)
			{
				throw new InvalidOperationException();
			}
			return _current ?? (_current = _largeCollection.Get(_id));
		}

		object IEnumerator.Current
		{
			get { return GetCurrent(); }
		}

		T IEnumerator<T>.Current
		{
			get { return GetCurrent(); }
		}

		bool IEnumerator.MoveNext()
		{
			checked
			{
				if (_id < -1)
				{
					throw new InvalidOperationException("Iterator Disposed");
				}
				if (_id > 1000000)
				{
					throw new InvalidOperationException("Long iteration!");
				}
				_current = null;
				_id++; // virtually unlimited. error if owerflow
				return true;
			}
		}

		void IEnumerator.Reset()
		{
			if (_id < 1)
			{
				throw new InvalidOperationException("Iterator Disposed");
			}
			_id = -1;
		}

		void IDisposable.Dispose()
		{
			_id = -2;
		}
	}
}
