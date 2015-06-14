using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NX.Internal
{
	[DataContract]
	[Serializable]
	public class NxObservableObject : INotifyPropertyChanged
	{
		[field: NonSerialized]
		static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _eventArgumentCache = new ConcurrentDictionary<string, PropertyChangedEventArgs>();

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
#if DEBUG
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (!string.IsNullOrEmpty(e.PropertyName)
				&& !string.Equals(e.PropertyName, "Item[]", StringComparison.Ordinal) // allow C# indexer
				)
			{
				if (TypeDescriptor.GetProperties(this)[e.PropertyName] == null)
				{
					var pros = TypeDescriptor.GetProperties(this);
					throw new Exception("Ivalid property name in OnPropertyChanged: " + e.PropertyName);
				}
			}
#endif
			var handler = PropertyChanged;
			if (handler != null)
			{
				// Debug.WriteLine("Notify: '" + e.PropertyName + "' at '" + GetType().Name + "'");
				handler(this, e);
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(_eventArgumentCache.GetOrAdd(propertyName ?? _null, x => new PropertyChangedEventArgs(x == _null ? null : x)));
		}

		private const string _null = "{5D53AA9F-A190-4F9B-97F5-EECFFB60937C}";

		protected void OnPropertyChanged(Expression<Func<object>> property)
		{
			OnPropertyChanged(Property.Name(property));
		}

		public override string ToString()
		{
			return GetType().Name;
		}
	}
}
