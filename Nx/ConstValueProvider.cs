using System;
using System.Collections.Generic;
using System.Linq;

namespace NX
{
	public static class ConstValueProvider
	{
		public static ConstValueProvider<T> Create<T>(T value)
		{
			return new ConstValueProvider<T>(value);
		}
	}

	public class ConstValueProvider<T> : IValueProvider<T>
	{
		private readonly T _value;

		public ConstValueProvider(T value)
		{
			_value = value;
		}

		object IValueProvider.Value
		{
			get
			{
				return Value;
			}
		}

		public T Value
		{
			get { return _value; }
		}

		public static implicit operator ConstValueProvider<T>(T value)
		{
			return new ConstValueProvider<T>(value);
		}

	}
}