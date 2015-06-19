using System;
using System.Collections.Generic;
using System.Linq;

namespace NX
{
	public enum SelectorBehaviour
	{
		/// <summary>
		/// Subscribe to source collection but do not subscribe to items and do not cache selections.
		/// </summary>
		None,
		/// <summary>
		/// Cache selections when each source should produce only one target. With caching there is no reason to subscribe and react to properties. If an element removed and then returned back to collection - it will reuse previous selector result.
		/// </summary> 
		/// <example>
		/// <code>
		/// models.SelectO(x => new ViewModel(x), SelectorBehaviour.Cache).
		/// </code>
		/// </example>
		Cache,
		/// <summary>
		/// Subscribe to collection item's INotifyPropertyChanged. When subscribed there is no reason to cache selections.
		/// </summary>
		/// <example>
		/// <code>
		/// models.SelectO(x => x.Pro, SelectorBehaviour.Listen).
		/// </code>
		/// </example>
		Listen,
	}
}