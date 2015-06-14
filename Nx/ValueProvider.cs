using System;
using System.Collections.Generic;
using System.Linq;
using NX.Internal;

namespace NX
{
	public static class ValueProvider
	{
/*
		public static ValueProvider<T> Create<T>(T value)
		{
			return new ValueProvider<T>(value);
		}

		public static IEnumerable<ValueProvider<T>> CreateCollection<T>(params T[] value)
		{
			return value.Select(x => new ValueProvider<T>(x));
		}
*/
	}

	public class ValueProvider<T> : NxObservableObject, IValueProvider<T>
	{
		public ValueProvider()
		{
		}

		public ValueProvider(T value)
		{
			_value = value;
		}

		private T _value;

		object IValueProvider.Value
		{
			get { return Value; }
		}

		public T Value
		{
			get { return _value; }
			set
			{
				if (!Equals(_value, value))
				{
					_value = value;
					OnPropertyChanged("Value");
				}
			}
		}

		public static implicit operator ValueProvider<T>(T value)
		{
			return new ValueProvider<T>(value);
		}

	}
}