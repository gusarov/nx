using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Nx.Tests.Annotations;

namespace Nx.Tests.Samples
{
	internal class Sample<T> : INotifyPropertyChanged
	{
		private T _data;

		public T Data
		{
			get { return _data; }
			set
			{
				if (Equals(value, _data))
				{
					return;
				}
				_data = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}