using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nx.Tests.Samples
{
	internal class EventLogger
	{
		public static readonly List<string> GlobalLog = new List<string>();
	}

	internal class EventLogger<T> : EventLogger
	{
	}

	internal class EventLogger<T, TE> : EventLogger<T>
	{
		private static int _nextId;

		private readonly T _obj;
		private readonly Func<TE, string> _logFormatter;

		public EventLogger(T obj, Func<TE, string> logFormatter = null)
		{
			_obj = obj;
			_logFormatter = logFormatter;
		}

		public readonly List<EventLogEntry<TE>> Log = new List<EventLogEntry<TE>>();

		public void Event(object sender, TE args)
		{
			Assert.AreEqual(_obj, sender);
			lock (Log)
			{
				var id = Interlocked.Increment(ref _nextId);
				Log.Add(new EventLogEntry<TE>
				{
					Id = id,
					Args = args,
				});
			}
			lock (GlobalLog)
			{
				GlobalLog.Add(_logFormatter == null ? args.ToString() : _logFormatter(args));
			}
		}
	}
}