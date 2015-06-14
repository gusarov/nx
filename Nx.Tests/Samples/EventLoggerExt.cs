using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Nx.Tests.Samples
{
	static class EventLoggerExt
	{
		public static EventLogger<T, PropertyChangedEventArgs> ListenProperty<T>(this T obj) where T : INotifyPropertyChanged
		{
			var logger = new EventLogger<T, PropertyChangedEventArgs>(obj);
			obj.PropertyChanged += logger.Event;
			return logger;
		}

		public static EventLogger<object, PropertyChangedEventArgs> ListenProperty2(this object obj)
		{
			var npc = obj as INotifyPropertyChanged;
			if (npc != null)
			{
				var logger = new EventLogger<object, PropertyChangedEventArgs>(npc);
				npc.PropertyChanged += logger.Event;
				return logger;
			}
			else
			{
				throw new Exception("object is not notifiable");
			}
		}

		public static EventLogger<T, NotifyCollectionChangedEventArgs> ListenCollection<T>(this T obj) where T : INotifyCollectionChanged
		{
			var logger = new EventLogger<T, NotifyCollectionChangedEventArgs>(obj);
			obj.CollectionChanged += logger.Event;
			return logger;
		}

		public static EventLogger<object, NotifyCollectionChangedEventArgs> ListenCollection2(this object obj)
		{
			var npc = obj as INotifyCollectionChanged;
			if (npc != null)
			{
				var logger = new EventLogger<object, NotifyCollectionChangedEventArgs>(npc);
				npc.CollectionChanged += logger.Event;
				return logger;
			}
			else
			{
				throw new Exception("collection is not notifiable");
			}
		}

		public static EventLogger<T, TE> Listen<T, TE>(this T obj, Action<T, Action<TE>> subscribe)
		{
			var logger = new EventLogger<T, TE>(obj);
			subscribe(obj, e => logger.Event(obj, e));
			return logger;
		}

	}
}