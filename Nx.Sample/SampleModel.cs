using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace Nx.Sample
{

	public class Book
	{
		private readonly int _id;

		public Book(int id)
		{
			_id = id;
		}

		public Book()
		{
			
		}

		public override string ToString()
		{
			return _id + " \t" + GetHashCode();
		}
	}

	class BookViewModel
	{
		private readonly Book _book;

		public BookViewModel(Book book)
		{
			_book = book;
		}

		public override string ToString()
		{
			return "VM: " + _book;
		}
	}

	public class BookStore
	{
		private readonly ExtremelyLargeCollection<Book> _allBooks = new ExtremelyLargeCollection<Book>();

		public IEnumerable<Book> Enumerable()
		{
			return _allBooks;
		}
	}
	/*
	public class InfiniteEnumerable<T> : IEnumerable<T>, ICollection<T>, IList<T> where T : class, ILargeEnumerableElement<T>
	{
		static InfiniteEnumerable()
		{
			Console.WriteLine();
			try
			{
				Activator.CreateInstance(typeof(T),true);
			}
			catch
			{
				
			}
		}

		public InfiniteEnumerable()
		{
			Console.WriteLine();
		}

		const int MaxIndex = int.MaxValue;

		private static Func<int, InfiniteEnumerable<T>, T> _factory;

		public static void RegisterFactory(Func<int, InfiniteEnumerable<T>, T> factory)
		{
			_factory = factory;
		}

		private readonly Dictionary<int, T> _cached = new Dictionary<int, T>();

		T GetOrAdd(int id)
		{
			T val;
			if (!_cached.TryGetValue(id, out val))
			{
				_cached[id] = val = _factory(_cached.Count, this);
			}
			return val;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return new MyEnumerator<T>(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public void Add(T item)
		{
			throw new NotSupportedException("Readonly!");
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
		public void Clear()
		{
			throw new NotSupportedException("Readonly!");
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		public bool Contains(T item)
		{
			return item.Parent == this;
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException("No one can create such a big array");
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(T item)
		{
			throw new NotSupportedException("Readonly!");
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public int Count
		{
			get { return MaxIndex; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </summary>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		public int IndexOf(T item)
		{
			throw new NotSupportedException("Very expensive!");
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
		public void Insert(int index, T item)
		{
			throw new NotSupportedException("Readonly!");
		}

		/// <summary>
		/// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
		public void RemoveAt(int index)
		{
			throw new NotSupportedException("Readonly!");
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>
		/// The element at the specified index.
		/// </returns>
		/// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
		public T this[int index]
		{
			get
			{
				return GetOrAdd(index);
			}
			set
			{
				throw new NotSupportedException("Readonly!");
			}
		}
	}

	class MyEnumerator<T> : IEnumerator<T> where T : class, ILargeEnumerableElement<T>
	{
		private readonly InfiniteEnumerable<T> _en;

		private State _currentState;
		private int _id;

		enum State
		{
			Initial,
			ValueExist,
			Disposed,
		}

		public MyEnumerator(InfiniteEnumerable<T> en)
		{
			_en = en;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_currentState = State.Disposed;
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public bool MoveNext()
		{
			_current = _en[_id++];
			_currentState = State.ValueExist;
			return true; // it virtually never ends
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public void Reset()
		{
			_id = 0;
			_currentState = State.Initial;
		}

		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the current position of the enumerator.
		/// </returns>
		public T Current
		{
			get
			{
				if (_currentState == State.ValueExist)
				{
					return _current;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		private T _current;

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		object IEnumerator.Current
		{
			get { return Current; }
		}
	}

	class MyQueriable<T> : IQueryable<T>
	{

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Type ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		Expression _expression;
		public System.Linq.Expressions.Expression Expression
		{
			get
			{
				return _expression;
			}
		}

		MyQueryProvider _provider;
		public IQueryProvider Provider
		{
			get
			{
				return _provider;
			}
		}
	}

	public class MyQueryProvider : IQueryProvider
	{
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			throw new NotImplementedException();
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public TResult Execute<TResult>(Expression expression)
		{
			throw new NotImplementedException();
		}

		public object Execute(Expression expression)
		{
			throw new NotImplementedException();
		}
	}
	 * */
}
