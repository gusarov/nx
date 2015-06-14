using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace NX.Internal
{
	abstract class SelfMaintainable
	{
		#region STATIC

		static readonly List<SelfMaintainable> _commonInstanties = new List<SelfMaintainable>();
		static readonly Timer _commonTimer;

		/// <summary>
		/// 10 Minutes
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly TimeSpan CommonIntervalDefault = TimeSpan.FromMinutes(10);

		static TimeSpan _commonInterval = TimeSpan.FromMinutes(10);

		static SelfMaintainable()
		{
			_commonTimer = new Timer(Tick, null, _commonInterval, _commonInterval);
		}

		/// <summary>
		/// Default interval for all SelfMaintainable instanties that not defines their interval explicitly
		/// </summary>
		[Obsolete("Use it for test purpose or for fine tuning only! Common Interval is 10 minutes by default. Consider 'Interval' proeprty instead for defining an instance specific interval")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static TimeSpan CommonInterval
		{
			get { return _commonInterval; }
			set { SetCommonInterval(value); }
		}

		static void SetCommonInterval(TimeSpan value)
		{
			if (value < TimeSpan.Zero)
			{
				// http://msdn.microsoft.com/en-us/library/317hx6fa.aspx
				value = TimeSpan.FromMilliseconds(-1); // disable
			}
			_commonInterval = value;
			_commonTimer.Change(_commonInterval, _commonInterval);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void CommonIntervalReset()
		{
			SetCommonInterval(CommonIntervalDefault);
		}

		static void Tick(object sender)
		{
			SelfMaintainable[] instanties;
			lock (_commonInstanties)
			{
				instanties = _commonInstanties.ToArray();
			}
			foreach (var instance in instanties)
			{
				instance.PerformMaintainance();
			}
		}

		#endregion

		Timer _specificTimer;
		TimeSpan? _specificInterval;

		public bool IsSpecific
		{
			get { return _specificInterval.HasValue; }
		}

		void SpecificTick(object sender)
		{
			Maintain();
		}

		protected SelfMaintainable()
			: this(false, null)
		{

		}

		protected SelfMaintainable(TimeSpan? specificMaintenanceInterval)
			: this(false, specificMaintenanceInterval)
		{

		}

		protected SelfMaintainable(bool maintainOnConstructor, TimeSpan? specificMaintenanceInterval)
		{
			SubscribeToCommon();
			Interval = specificMaintenanceInterval;
			if (maintainOnConstructor)
			{
				PerformMaintainanceAsync();
			}
		}

		void SubscribeToCommon()
		{
			lock (_commonInstanties)
			{
				_commonInstanties.Add(this);
			}
		}

		void UnsubscribeFromCommon()
		{
			lock (_commonInstanties)
			{
				_commonInstanties.Remove(this);
			}
		}


		public bool IsScheduled
		{
			get { return Interval > default(TimeSpan); }
		}

		/// <summary>
		/// Maintainance interval specific to this instance
		/// </summary>
		public TimeSpan? Interval
		{
			get { return _specificInterval ?? _commonInterval; }
			set
			{
				if (value.HasValue)
				{
					// use specific
					lock (_configSync)
					{
						UnsubscribeFromCommon();
						if (_specificTimer == null)
						{
							_specificTimer = new Timer(SpecificTick, null, value.Value, value.Value);
						}
						else
						{
							_specificTimer.Change(value.Value, value.Value);
						}
						_specificInterval = value;
					}
				}
				else
				{
					if (IsSpecific)
					{
						lock (_configSync)
						{
							// use common
							if (_specificTimer != null)
							{
								_specificTimer.Change(Timeout.Infinite, Timeout.Infinite); // disable
								// lets just leave this timer for latter reuse...
							}
							_specificInterval = null;
							SubscribeToCommon();
						}
					}
				}
			}
		}

		readonly object _maintainanceSync = new object();
		readonly object _configSync = new object();

		public void PerformMaintainance()
		{
			lock (_maintainanceSync)
			{
				Maintain();
			}
		}

		public void PerformMaintainanceAsync()
		{
			ThreadPool.QueueUserWorkItem(x => PerformMaintainance());
		}

		/// <summary>
		/// Core method for performing maintenance. Method is synchronized.
		/// </summary>
		protected abstract void Maintain();

	}
}