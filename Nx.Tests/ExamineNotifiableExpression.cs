using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NX;
using Nx.Tests.Samples;

namespace Nx.Tests
{
	[TestClass]
	public class ExamineNotifiableExpression
	{
		[TestMethod]
		public void Should_notify_when_expression_result_changed()
		{
			var data = new ObservableCollection<int>();
			var sum = data.SumO();
			Assert.IsNotNull(sum, "SumO returns null");
			Assert.IsNotNull(sum as INotifyPropertyChanged, "SumO returns not notifiable object");
			var log = new List<string>();
			((INotifyPropertyChanged)sum).PropertyChanged += (s, e) => log.Add(e.PropertyName + " " + sum.Value);
			Assert.AreEqual(0, sum.Value);

			data.Add(1);
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("Value 1", log[0]);
			Assert.AreEqual(1, sum.Value);

			data.Add(2);
			Assert.AreEqual(2, log.Count);
			Assert.AreEqual("Value 3", log[1]);
			Assert.AreEqual(3, sum.Value);
		}

		[TestMethod]
		public void Should_notify_when_expression_result_changed_for_fp_collection()
		{
			var data = new ObservableCollection<IValueProvider<int>>();
			var sum = data.SumO();
			Assert.IsNotNull(sum, "SumO returns null");
			Assert.IsNotNull(sum as INotifyPropertyChanged, "SumO returns not notifiable object");
			var log = new List<string>();
			((INotifyPropertyChanged)sum).PropertyChanged += (s, e) => log.Add(e.PropertyName + " " + sum.Value);
			Assert.AreEqual(0, sum.Value);

			data.Add(ConstValueProvider.Create(1));
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("Value 1", log[0]);
			Assert.AreEqual(1, sum.Value);

			data.Add(ConstValueProvider.Create(2));
			Assert.AreEqual(2, log.Count);
			Assert.AreEqual("Value 3", log[1]);
			Assert.AreEqual(3, sum.Value);
		}

	}

	[TestClass]
	public class ExamineTransformations
	{
		[TestInitialize]
		public void Init()
		{
			EventLogger.GlobalLog.Clear();
		}

		[TestMethod]
		public void Should_select()
		{
			var src = new ObservableCollection<int>(new[]{1, 2, 3});
			var expLogCol = src.ListenCollection();
			var expLogPro = src.ListenProperty();

			var trg = src.SelectO(x => x.ToString());
			var actLogCol = trg.ListenCollection2();
			var actLogPro = trg.ListenProperty2();

			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, trg.ToArray());
			Assert.AreEqual(0, EventLogger.GlobalLog.Count);

			src.Add(4);
			Assert.AreEqual(expLogCol.Log.Count, actLogCol.Log.Count);
			for (int i = 0; i < expLogCol.Log.Count; i++)
			{
				Assert.AreEqual(expLogCol.Log[i].Args.Action, actLogCol.Log[i].Args.Action);
				Assert.AreEqual(expLogCol.Log[i].Args.OldItems, actLogCol.Log[i].Args.OldItems);
				Assert.AreEqual(expLogCol.Log[i].Args.OldStartingIndex, actLogCol.Log[i].Args.OldStartingIndex);
				Assert.AreEqual(expLogCol.Log[i].Args.NewStartingIndex, actLogCol.Log[i].Args.NewStartingIndex);
				CollectionAssert.AreEqual(new[]{"4"}, actLogCol.Log[i].Args.NewItems);
			}
			Assert.AreEqual(1, actLogPro.Log.Count);
			Assert.AreEqual("Count", actLogPro.Log[0].Args.PropertyName);
			CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, trg.ToArray());
		}

		[TestMethod]
		public void Should_select_from_vp_to_simple_col()
		{
			var src = new ObservableCollection<ValueProvider<int>>(CreateVpCollection(1, 2, 3));
			var source = (IEnumerable<IValueProvider<int>>)src;
			var target = source.SelectO(x => x.Value, SelectorBehaviour.Listen);
			var sum = target.SumO();

			var targetLogCol = target.ListenCollection2();
			var targetLogPro = target.ListenProperty2();
			var sumLogPro = sum.ListenProperty2();

			CollectionAssert.AreEqual(new[] {1, 2, 3}, target.ToArray());
			Assert.AreEqual(6, sum.Value);

			src.Add(CreateVp(4));

			Assert.AreEqual(1, targetLogCol.Log.Count);
			Assert.AreEqual(NotifyCollectionChangedAction.Add, targetLogCol.Log[0].Args.Action);
			Assert.AreEqual(null, targetLogCol.Log[0].Args.OldItems);
			Assert.AreEqual(-1, targetLogCol.Log[0].Args.OldStartingIndex);
			Assert.AreEqual(3, targetLogCol.Log[0].Args.NewStartingIndex);
			Assert.AreEqual(1, targetLogCol.Log[0].Args.NewItems.Count);
			Assert.AreEqual(4, targetLogCol.Log[0].Args.NewItems[0]);
			
			Assert.AreEqual(1, targetLogPro.Log.Count);
			Assert.AreEqual("Count", targetLogPro.Log[0].Args.PropertyName);
			Assert.AreEqual(1, sumLogPro.Log.Count);
			Assert.AreEqual("Value", sumLogPro.Log[0].Args.PropertyName);

			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, target.ToArray());
			Assert.AreEqual(10, sum.Value);

			// clear
			EventLogger.GlobalLog.Clear();
			targetLogCol.Log.Clear();
			targetLogPro.Log.Clear();
			sumLogPro.Log.Clear();

			src[1].Value = -2;

			var temp = new ObservableCollection<int>(new[] {1, 2, 3});
			var expectedReplace = temp.ListenCollection();
			temp[1] = -2;
			Assert.AreEqual(1, expectedReplace.Log.Count);
			Assert.AreEqual(NotifyCollectionChangedAction.Replace, expectedReplace.Log[0].Args.Action);

			Assert.AreEqual(1, targetLogCol.Log.Count);
			Assert.AreEqual(NotifyCollectionChangedAction.Replace, targetLogCol.Log[0].Args.Action);
			Assert.AreEqual(1, targetLogCol.Log[0].Args.OldStartingIndex);
			Assert.AreEqual(1, targetLogCol.Log[0].Args.NewStartingIndex);
			Assert.AreEqual(2, targetLogCol.Log[0].Args.OldItems[0]);
			Assert.AreEqual(-2, targetLogCol.Log[0].Args.NewItems[0]);

			Assert.AreEqual(0, targetLogPro.Log.Count); // even 'Count' is not changed after replace
			Assert.AreEqual(1, sumLogPro.Log.Count);
			Assert.AreEqual("Value", sumLogPro.Log[0].Args.PropertyName);

			CollectionAssert.AreEqual(new[] { 1, -2, 3, 4 }, target.ToArray());
			Assert.AreEqual(6, sum.Value);
		}

		[TestMethod]
		public void Should_not_subscribe_for_property_changed_if_not_necessery()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Should_cache_selector()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Should_reset_incrementally()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Should_add_several_items_as_one_operation()
		{
			var list = new List<int> { 1, 2, 3 };
			var notifier = new CollectionNotifier<int>(list);
			var target = notifier.SelectO(x => x.ToString());
			var logger = target.ListenCollection2();

			list.Add(4);
			list.Add(5);
			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, target.ToArray());

			notifier.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { 4, 5 }, 3));
			CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "5" }, target.ToArray());

			Assert.AreEqual(1, logger.Log.Count);
		}

		[TestMethod]
		public void Should_remove_several_items_as_one_operation()
		{
			var list = new List<int> { 1, 2, 3, 4, 5 };
			var notifier = new CollectionNotifier<int>(list);
			var target = notifier.SelectO(x => x.ToString());
			var logger = target.ListenCollection2();

			list.Remove(4);
			list.Remove(5);
			CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "5" }, target.ToArray());

			notifier.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { 4, 5 }, 3));
			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, target.ToArray());

			Assert.AreEqual(1, logger.Log.Count);
		}

		public static IEnumerable<ValueProvider<T>> CreateVpCollection<T>(params T[] value)
		{
			return value.Select(x => new ValueProvider<T>(x));
		}

		public static ValueProvider<T> CreateVp<T>(T value)
		{
			return new ValueProvider<T>(value);
		}

	}
}
