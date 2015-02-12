﻿using System;

namespace SIL.WritingSystems
{
	internal class SimpleMonitor : IDisposable
	{
		private bool _busy;

		public SimpleMonitor Enter()
		{
			_busy = true;
			return this;
		}

		public void Dispose()
		{
			_busy = false;
		}

		public bool Busy
		{
			get { return _busy; }
		}
	}
}