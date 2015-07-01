using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	internal abstract class NxObservableReadonlyList<TR> : NxObservableEnumerable<TR>, IList, IList<TR>
	{
		protected virtual void CopyToCore(Array array, int index)
		{
			throw new NotImplementedException();
		}

		protected virtual void CopyToCore(TR[] array, int index)
		{
			CopyToCore((Array)array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyToCore(array, index);
		}

		object ICollection.SyncRoot
		{
			get { throw new NotSupportedException(); }
		}

		bool ICollection<TR>.Remove(TR item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<TR>.IsReadOnly
		{
			get { return true; }
		}


		#region IList Members

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			return Contains((TR)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((TR)value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
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
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			get { return Get(index); }
			set
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#region ICollection Members

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region IList<TR> Members

		public virtual int IndexOf(TR item)
		{
			// this is enough for WPF virtualization and costs nothing
			if (Equals(Get(0), item))
			{
				return 0;
			}
			return IndexOfCore(item);
		}

		protected virtual int IndexOfCore(TR item)
		{
			throw new NotImplementedException();
		}

		void IList<TR>.Insert(int index, TR item)
		{
			throw new NotSupportedException();
		}

		void IList<TR>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		protected abstract TR Get(int index);

		TR IList<TR>.this[int index]
		{
			get { return Get(index); }
			set
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#region ICollection<TR> Members

		void ICollection<TR>.Add(TR item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TR>.Clear()
		{
			throw new NotSupportedException();
		}

		public virtual bool Contains(TR item)
		{
			return IndexOf(item) >= 0;
		}

		void ICollection<TR>.CopyTo(TR[] array, int arrayIndex)
		{
			CopyToCore(array, arrayIndex);
		}

		#endregion

		public abstract int Count { get; }
	}
}