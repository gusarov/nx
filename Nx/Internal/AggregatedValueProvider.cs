using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace NX.Internal
{
	abstract class AggregatedValueProvider<TR, TS> : NxObservableObject, IValueProvider<TR>
	{
		private static readonly bool _isReferenceType = !typeof(TS).IsValueType;

		private readonly IEnumerable<TS> _operands;
		private readonly HashSet<TS> _subscribed = _isReferenceType ? new HashSet<TS>() : null;

		protected AggregatedValueProvider(IEnumerable<TS> operands)
		{
			_operands = operands;
			if (_isReferenceType)
			{
				foreach (var valueProvider in _operands)
				{
					Subscribe(valueProvider);
				}
			}
			var ncc = operands as INotifyCollectionChanged;
			if (ncc != null)
			{
				ncc.CollectionChanged += CollectionChanged;
			}
			Reset();
		}

		void Unsubscribe(TS item)
		{
			var npc = item as INotifyPropertyChanged;
			if (npc != null)
			{
				npc.PropertyChanged -= ValueProviderPropertyChanged;
				_subscribed.Remove(item);
			}
		}

		void Subscribe(TS item)
		{
			var npc = item as INotifyPropertyChanged;
			if (npc != null)
			{
				if (_subscribed.Add(item))
				{
					npc.PropertyChanged += ValueProviderPropertyChanged;
				}
			}
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				{
					if (_isReferenceType)
					{
						Subscribe((TS)e.NewItems[0]);
					}
					Add((TS)e.NewItems[0]);
					break;
				}
				case NotifyCollectionChangedAction.Remove:
				{
					var old = (TS)e.OldItems[0];
					if (_isReferenceType)
					{
						Unsubscribe(old);
					}
					Remove(old);
					break;
				}
				case NotifyCollectionChangedAction.Replace:
				{
					// hack - non additive aggregates should complete replace with single operation
					Remove((TS)e.OldItems[0]);
					Add((TS)e.NewItems[0]);
					break;
				}
				case NotifyCollectionChangedAction.Move:
					throw new NotSupportedException();
				case NotifyCollectionChangedAction.Reset:
				{
					if (_isReferenceType)
					{
						foreach (var valueProvider in _subscribed.ToArray())
						{
							Unsubscribe(valueProvider);
						}
						foreach (var valueProvider in _operands)
						{
							Subscribe(valueProvider);
						}
					}
					Reset();
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void ValueProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Recheck((TS)sender);
		}

		protected virtual void Remove(TS old)
		{
			Reset();
		}

		protected virtual void Add(TS newItem)
		{
			Reset();
		}

		protected virtual void Reset()
		{
			SetValue(Calculate(_operands));
		}

		protected virtual void Recheck(TS item)
		{
			Reset();
		}

		protected abstract TR Calculate(IEnumerable<TS> source);

		#region Value

		private TR _lastReport;
		private bool _anythingReported;

		void SetValue(TR value)
		{
			if (!_anythingReported || !Equals(_lastReport, value))
			{
				_lastReport = value;
				_anythingReported = true;
				OnPropertyChanged(() => Value);
			}
		}

		object IValueProvider.Value
		{
			get
			{
				return Value;
			}
		}

		public TR Value
		{
			get
			{
				if (!_anythingReported)
				{
					_lastReport = Calculate(_operands);
					_anythingReported = true;
				}
				return _lastReport;
			}
		}

		#endregion
	}
}