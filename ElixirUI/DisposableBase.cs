using System;

namespace ElixirUI
{
	public abstract class DisposableBase : IDisposable
	{
		public bool IsDisposed { get; set; } = false;

		protected abstract void DoCleanup();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bIsCleanupNeeded)
		{
			if (IsDisposed) return;

			if (bIsCleanupNeeded)
			{
				DoCleanup();
			}

			IsDisposed = true;
		}
	}
}
