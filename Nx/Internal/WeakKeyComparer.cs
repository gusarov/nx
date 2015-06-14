using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	sealed class WeakKeyComparer : IEqualityComparer<EquatableWeakReference>
	{
		static readonly WeakKeyComparer _comparer = new WeakKeyComparer();
		public static WeakKeyComparer Comparer { get { return _comparer; } }

		WeakKeyComparer()
		{

		}

		bool IEqualityComparer<EquatableWeakReference>.Equals(EquatableWeakReference x, EquatableWeakReference y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (ReferenceEquals(x, null))
			{
				return ReferenceEquals(y, null);
			}
			if (ReferenceEquals(y, null))
			{
				return false;
			}
			if (x.GetHashCode() != y.GetHashCode())
			{
				return false;
			}
			//			var wx = x as WeakReference;
			//			var wy = y as WeakReference;

			var ax = x.Target;
			var ay = y.Target;

			if (ReferenceEquals(ax, null))
			{
				return ReferenceEquals(ay, null);
			}

			if (ReferenceEquals(ay, null))
			{
				return false;
			}

			return Equals(ax, ay);
		}

		int IEqualityComparer<EquatableWeakReference>.GetHashCode(EquatableWeakReference ewr)
		{
			return ewr.GetHashCode();
		}
	}
}