using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace NX.Internal
{
	internal abstract class NxObservableEnumerable<TR> : NxObservableObject, INotifyCollectionChanged, IEnumerable<TR>
	{
		// reduce count of objects allocated
		protected static readonly NotifyCollectionChangedEventArgs _reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected void OnCollectionChangedReset()
		{
			OnCollectionChanged(_reset);
		}

		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			var handler = CollectionChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		[Conditional("DEBUG")]
		protected void Validate(bool condition, string failMessage)
		{
			if (!condition)
			{
				throw new ObservableEnumerableException(failMessage);
			}
		}

		public abstract IEnumerator<TR> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}
}