using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nx.Sample
{
	static class LargeEnumerable
	{
		static IEnumerable<int> _allInt;
		public static IEnumerable<int> AllInt
		{
			get { return _allInt ?? (_allInt = new LargeIntEnumerable()); }
		}
	}

	class LargeIntEnumerable : IEnumerable<int>, IList, IList<int>, ICollection, ICollection<int>
	{
		#region IEnumerable<int> Members

		public IEnumerator<int> GetEnumerator()
		{
			for (int i = 0; i < int.MaxValue; i++)
			{
				yield return i;
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ICollection<int> Members

		public void Add(int item)
		{
			throw new NotSupportedException();
		}

		public int Add(object value)
		{
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			return value is int;
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			return (int)value;
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(int item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, int item)
		{
			throw new NotImplementedException();
		}

		void IList<int>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public int this[int index]
		{
			get { return index; }
			set { throw new NotImplementedException(); }
		}

		void IList.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		object IList.this[int index]
		{
			get { return index; }
			set { throw new NotImplementedException(); }
		}

		public bool Contains(int item)
		{
			return true;
		}

		public void CopyTo(int[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				return int.MaxValue;
			}
		}

		public object SyncRoot
		{
			get { throw new NotSupportedException(); }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(int item)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
