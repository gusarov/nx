using System;
using System.Collections.Generic;
using System.Linq;

namespace Nx.Tests.Samples
{
	class EventLogEntry<TE>
	{
		public int Id;
		public TE Args;
	}
}