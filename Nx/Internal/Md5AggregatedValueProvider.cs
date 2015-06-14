using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace NX.Internal
{
	internal class Md5AggregatedValueProvider : AggregatedValueProvider<byte[], int>
	{
		public Md5AggregatedValueProvider(IEnumerable<int> operands)
			: base(operands)
		{
		}

		protected override byte[] Calculate(IEnumerable<int> source)
		{
			var buf = source.SelectMany(BitConverter.GetBytes).ToArray();
			var md5 = MD5.Create();
			return md5.ComputeHash(buf);
		}
	}
}