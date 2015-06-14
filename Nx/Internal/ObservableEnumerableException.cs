using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace NX.Internal
{
	[Serializable]
	public class ObservableEnumerableException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ObservableEnumerableException()
		{
		}

		public ObservableEnumerableException(string message) : base(message)
		{
		}

		public ObservableEnumerableException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ObservableEnumerableException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}