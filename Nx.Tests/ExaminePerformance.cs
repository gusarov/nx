using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NX;

namespace Nx.Tests
{
	[TestClass]
	public class ExaminePerformance
	{
		private void PerformanceAssert(Action<int> action)
		{
			PerformanceAssert(null, action);
		}

		private void PerformanceAssert(Action<int> action, double? iterationsPerSecondExpected)
		{
			PerformanceAssert(iterationsPerSecondExpected, action);
		}

		private void PerformanceAssert(double? iterationsPerSecondExpected, Action<int> action)
		{
			var ips = PerformanceMeasure(action);
			var msg = string.Format("{0:n0} ips ({1:p0} of {2:n0} expected)", ips, ips / iterationsPerSecondExpected, iterationsPerSecondExpected);
			if (iterationsPerSecondExpected.HasValue)
			{
				Assert.IsTrue(ips >= iterationsPerSecondExpected, msg);
				Console.WriteLine(msg);
			}
			else
			{
				Assert.Inconclusive(msg);
			}
		}

		private const int _measureTime = 1000; // run for one second
		private const int _chunk = 1000;
		/// <summary>
		/// This is max counter used for measurment. There is no reason in very big numbers, couple millions is enougth to fit them into second. And if that number used for List&lt;int&gt; it better be allocatable
		/// </summary>
		private const int _max = int.MaxValue / 5;
		private const int _maxLastChunk = _max - _chunk;

		double PerformanceMeasure(Action<int> act)
		{
			var sw = Stopwatch.StartNew();
			int c = 0;
			while (sw.ElapsedMilliseconds < _measureTime)
			{
				for (int i = 0; i < _chunk; i++)
				{
					act(c++);
				}
				if (c > _maxLastChunk)
				{
					break;
				}
			}
			sw.Stop();
			return c / sw.Elapsed.TotalSeconds;
		}

/*
		[TestMethod]
		public void Perf05_AllocateList()
		{
			var src1 = new List<int>(_max);
		}

		[TestMethod]
		public void Perf05_ListAdd()
		{
			var src1 = new List<int>();
			PerformanceAssert(10000000, src1.Add);
		}

		[TestMethod]
		public void Perf05_ListPreallocatedAdd()
		{
			var src1 = new List<int>(_max);
			PerformanceAssert(10000000, src1.Add);
		}

		[TestMethod]
		public void Perf05_ArrayListAdd()
		{
			var src2 = new ArrayList();
			MeasureAssert(10000000, i => src2.Add(i));
		}

		[TestMethod]
		public void Perf05_ArrayListPreallocatedAdd()
		{
			var src2 = new ArrayList(_intMaxValueLastChunk / 10);
			MeasureAssert(10000000, i => src2.Add(i));
		}

		[TestMethod]
		public void Perf10_ObservableCollectionAdd()
		{
			var src = new ObservableCollection<int>();
			PerformanceAssert(2000000, src.Add);
		}

		[TestMethod]
		public void Perf20_ObservableCollectionAddRef()
		{
			var src = new ObservableCollection<ConstValueProvider<int>>();
			PerformanceAssert(1000000, x => src.Add(x));
		}
*/

		[TestMethod]
		public void Perf30_SelectAdd()
		{
			var src = new ObservableCollection<int>();
			var trg = src.SelectO(x => x.ToString());
			PerformanceAssert(500000, src.Add);
		}

		[TestMethod]
		public void Perf40_SelectAddRef()
		{
			var src = new ObservableCollection<ConstValueProvider<int>>();
			var trg = src.SelectO(x => x.ToString());
			PerformanceAssert(500000, x => src.Add(x));
		}

		[TestMethod]
		public void Perf50_SelectAddListen()
		{
			var src = new ObservableCollection<ConstValueProvider<int>>();
			var trg = src.SelectO(x => x.ToString(), SelectorBehaviour.Listen);
			PerformanceAssert(500000, x => src.Add(x));
		}

		[TestMethod]
		public void Perf60_SelectAddRefCache()
		{
			var src = new ObservableCollection<ConstValueProvider<int>>();
			var trg = src.SelectO(x => x.Value.ToString(), SelectorBehaviour.Cache);
			PerformanceAssert(100000, x => src.Add(x));
		}

		[TestMethod]
		public void Perf70_SelectUpdate()
		{
			var m = 1000000;
			var src = new ObservableCollection<ValueProvider<int>>(Enumerable.Range(0, m).Select(x => (ValueProvider<int>)x));
			var rnd = new Random();
			var trg = src.SelectO(x => x.Value.ToString());
			PerformanceAssert(500000, i => src[rnd.Next(m)].Value = i);
		}

		[TestMethod]
		public void Perf80_SelectUpdateListen()
		{
			var m = 1000000;
			var src = new ObservableCollection<ValueProvider<int>>(Enumerable.Range(0, m).Select(x => (ValueProvider<int>)x));
			var trg = src.SelectO(x => x.Value.ToString(), SelectorBehaviour.Listen);
			var rnd = new Random();
			PerformanceAssert(200000, x => src[rnd.Next(m)].Value = x);

			// check results
			var srcArr = src.ToArray();
			var trgArr = trg.ToArray();
			Assert.AreEqual(srcArr.Length, trgArr.Length);
			for (int i = 0, c = srcArr.Length; i < c; i++)
			{
				Assert.AreEqual(srcArr[i].Value.ToString(), trgArr[i]);
			}
		}
	}
}