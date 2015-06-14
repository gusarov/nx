using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace NX.Internal
{
	internal class Md5AggregatedValueProviderFromVP : AggregatedValueProviderFromVP<byte[], int>
	{
		public Md5AggregatedValueProviderFromVP(IEnumerable<IValueProvider<int>> operands)
			: base(operands)
		{
		}

		protected override byte[] Calculate(IEnumerable<IValueProvider<int>> source)
		{
			var buf = source.Select(x=>x.Value).SelectMany(BitConverter.GetBytes).ToArray();
			var md5 = MD5.Create();
			return md5.ComputeHash(buf);
		}
	}
}