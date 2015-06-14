using System;
using System.Collections.Generic;
using System.Linq;

namespace NX.Internal
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
	class EquatableWeakReference : WeakReference, IEquatable<EquatableWeakReference>
	{
		readonly int _hashCode;

		public EquatableWeakReference(object obj)
			: base(obj)
		{
			_hashCode = obj.GetHashCode();
		}

		bool IEquatable<EquatableWeakReference>.Equals(EquatableWeakReference other)
		{
			return Equals(other);
		}

		public override bool Equals(object other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			if (other.GetHashCode() != _hashCode)
			{
				return false;
			}
			var aliveTarget = Target;
			return ReferenceEquals(other, this) || ReferenceEquals(other, aliveTarget);
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}
	}
}