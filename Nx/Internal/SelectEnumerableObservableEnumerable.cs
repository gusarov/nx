using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace NX.Internal
{
	internal sealed class SelectObservableEnumerable<TSource, TResult> : NxObservableEnumerable, ICollection<TResult>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly IEnumerable<TSource> _source;
		private readonly Func<TSource, TResult> _selector;
		private readonly List<TResult> _target = new List<TResult>(8);

		private readonly bool _listenElement;
		private readonly WeakKeyCache<TSource, TResult> _cache; // set holds weak reference to source items in order to reuse old selection if item returns back.
		private readonly Dictionary<TSource, int> _map; // set corresponds to set of source items
		private readonly List<TSource> _sourceList; // list corresponds to set of source items

		internal SelectObservableEnumerable(IEnumerable<TSource> source, Func<TSource, TResult> selector, SelectorBehaviour selectorBehaviour)
		{
			switch (selectorBehaviour)
			{
				case SelectorBehaviour.None:
					break;
				case SelectorBehaviour.Cache:
					_cache = new WeakKeyCache<TSource, TResult>();
					break;
				case SelectorBehaviour.Listen:
					_listenElement = true;
					break;
				default:
					throw new ArgumentOutOfRangeException("selectorBehaviour");
			}
			if (_listenElement)
			{
				_map = new Dictionary<TSource, int>();
				_sourceList = new List<TSource>();
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			_source = source;
			_selector = selector;
			var ncc = source as INotifyCollectionChanged;
			if (ncc != null)
			{
				ncc.CollectionChanged += ncc_CollectionChanged;
			}
			DoReset(true);
		}

		void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex == -1)
					{
						throw new ObservableEnumerableException(string.Format("Source collection '{0}' notifies about '{1}' but no NewStartingIndex provided", _source.GetType().Name, e.Action));
					}
					if (e.NewItems.Count > 1)
					{
						DoAdd(e.NewItems, e.NewStartingIndex);
					}
					else if (e.NewItems.Count == 1) // speedup
					{
						DoAdd((TSource)e.NewItems[0], e.NewStartingIndex);
					}
					else
					{
						throw new ObservableEnumerableException(string.Format("Source collection '{0}' notifies about '{1}' but no NewItems provided", _source.GetType().Name, e.Action));
					}
					OnPropertyChanged("Count");
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex == -1)
					{
						throw new ObservableEnumerableException(string.Format("Source collection '{0}' notifies about '{1}' but no OldStartingIndex provided", _source.GetType().Name, e.Action));
					}
					if (e.OldItems.Count > 1)
					{
						DoRemove(e.OldItems, e.OldStartingIndex);
					}
					else if (e.OldItems.Count == 1) // speedup
					{
						DoRemove((TSource)e.OldItems[0], e.OldStartingIndex);
					}
					else
					{
						throw new ObservableEnumerableException(string.Format("Source collection '{0}' notifies about '{1}' but no OldItems provided", _source.GetType().Name, e.Action));
					}
					OnPropertyChanged("Count");
					break;
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					throw new NotSupportedException(string.Format("Source collection '{0}' notifies about '{1}' but it is not supported", _source.GetType().Name, e.Action));
				case NotifyCollectionChangedAction.Reset:
					DoReset();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UnsubscribeRemove(TSource item, int index)
		{
			Unsubscribe(item);
			_target.RemoveAt(index);
			if (_sourceList != null)
			{
				_sourceList.RemoveAt(index);
				for (int i = index; i < _sourceList.Count; i++)
				{
					_map[_sourceList[i]] = i;
				}
			}
		}

		void Unsubscribe(TSource item)
		{
			if (_listenElement)
			{
				var npc = item as INotifyPropertyChanged;
				if (npc != null)
				{
					if (_map.Remove(item))
					{
						npc.PropertyChanged -= ValueProviderPropertyChanged;
					}
				}
			}
		}

		private void Subscribe(TSource item, TResult target, int index)
		{
			if (_listenElement)
			{
				var npc = item as INotifyPropertyChanged;
				if (npc != null) // only notifiable stuff really required in map
				{
					//if (!_map.ContainsKey(item))
					{
						_map.Add(item, index);
						npc.PropertyChanged += ValueProviderPropertyChanged;
					}
				}
			}
		}

		private void SubscribeInsert(TSource item, TResult target, int index)
		{
			Subscribe(item, target, index);
			_target.Insert(index, target);
			if (_sourceList != null)
			{
				_sourceList.Insert(index, item);
				for (int i = index + 1; i < _sourceList.Count; i++)
				{
					_map[_sourceList[i]] = i;
				}
			}
		}

		void ValueProviderPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			// We cannot dig into selector byte code for now. Just try to update
			var src = (TSource)sender;
			DoUpdate(src);
		}

		private TResult Select(TSource source)
		{
			if (_cache != null)
			{
				return _cache[source, x => _selector(x)];
			}
			return _selector(source);
		}

		private void DoUpdate(TSource source)
		{
//#warning Significant Performance Penalty here because of IndexOf
			var index = _map[source];
			var target = _target[index];
			var newSelection = Select(source);
			if (!Equals(newSelection, target))
			{
				_target[index] = newSelection;
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newSelection, target, index));
			}
		}

		private void DoAdd(IList newItems, int index)
		{
			var c = newItems.Count;
			var src = new TSource[c];
			var res = new TResult[c];
			for (int i = 0; i < c; i++)
			{
				var item = (TSource)newItems[i];
				var re = Select(item);
				res[i] = re;
				src[i] = item;
				Subscribe(item, re, index + i);
			}
			_target.InsertRange(index, res);
			if (_sourceList != null)
			{
				_sourceList.InsertRange(index, src);
				for (int i = index + src.Length; i < _sourceList.Count; i++)
				{
					_map[_sourceList[i]] = i;
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, res, index));
		}

		private void DoAdd(TSource newItem, int index)
		{
			var re = Select(newItem);
			SubscribeInsert(newItem, re, index);
			Validate();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, re, index));
		}

		private void DoRemove(IList oldItems, int index)
		{
			var c = oldItems.Count;
			var res = new TResult[c];
			for (int i = c - 1; i >= 0; i--)
			{
				var oldItem = (TSource)oldItems[i];
				TResult targetItem;
				res[i] = targetItem = _target[index + i];
				Validate(oldItem, targetItem);
				Unsubscribe(oldItem);
			}
			_target.RemoveRange(index, oldItems.Count);
			if (_sourceList != null)
			{
				_sourceList.RemoveRange(index, oldItems.Count);
				for (int i = index; i < _sourceList.Count; i++)
				{
					_map[_sourceList[i]] = i;
				}
			}
			Validate();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, res, index));
		}

		private void DoRemove(TSource oldItem, int index)
		{
			var oldTarget = _target[index];
			UnsubscribeRemove(oldItem, index);
			Validate(oldItem, oldTarget);
			Validate();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldTarget, index));
		}

		private void DoReset(bool isInitial = false)
		{
			if (!isInitial)
			{
				_target.Clear();
				if (_map.Count > 0)
				{
					foreach (var source in _map.Keys)
					{
						Unsubscribe(source);
					}
					_map.Clear();
				}
			}
			int i = 0;
			var src = new List<TSource>();
			var trg = new List<TResult>();
			foreach (var item in _source)
			{
				src.Add(item);
				var target = Select(item);
				trg.Add(target);
				Subscribe(item, target, i++);
			}
			_target.AddRange(trg);
			if (_sourceList != null)
			{
				_sourceList.AddRange(src);
			}
			if (!isInitial)
			{
				OnCollectionChangedReset();
				OnPropertyChanged("Count");
			}

			Validate();
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			return _target.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<TResult>.Add(TResult item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TResult>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(TResult item)
		{
			return _target.Contains(item);
		}

		public void CopyTo(TResult[] array, int arrayIndex)
		{
			_target.CopyTo(array, arrayIndex);
		}

		bool ICollection<TResult>.Remove(TResult item)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get { return _target.Count; }
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[Conditional("DEBUG")]
		private void Validate()
		{
			Validate(_source.Count() == _target.Count, "Source and target collections count missmatch.");
			if (_map != null && _sourceList != null)
			{
				Validate(_sourceList.Count() == _target.Count, "Source and target collections count missmatch.");
				for (int i = 0; i < _sourceList.Count; i++)
				{
					Validate(_map[_sourceList[i]] == i, "Index in map entry missmatch");
				}
			}
		}

		[Conditional("DEBUG")]
		private void Validate(TSource src, TResult trg)
		{
			Validate(Equals(Select(src), trg), "Translating source to target do not match to current target. Selectors should be reentrable.");
		}

	}
}