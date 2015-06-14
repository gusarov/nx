using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NX
{
	public interface IValueProvider // : INotifyPropertyChanged
	{
		object Value { get; }
	}

	public interface IValueProvider<out T> : IValueProvider
	{
		new T Value { get; }
	}

}