using System.Timers;

namespace ElixirUI
{
	/// <summary>
	/// IO handler meant to run in a separate thread from the UI.  Controls sending and receiving from the Elixir port.
	/// </summary>
	public class IOThread : DisposableBase
	{
		private const int iTimerInterval = 250;

		private readonly Port port = new();
		private readonly Timer timer = new();

		public IOThread()
		{
			timer.Elapsed += Timer_Elapsed;
			timer.Interval = iTimerInterval;
			timer.Start();
		}

		// Communicates with the Elixir port every tick
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			GetServerResponse();
			SendUserInput();
		}

		// Reads a value from the Elixir port and writes it to the ServerResponses collection.
		private void GetServerResponse()
		{
			string strResults = port.Read();

			if (false == string.IsNullOrWhiteSpace(strResults))
			{
				Statics.ServerResponses.Add(strResults);
			}
		}

		// Attempts to take one entry from the UserInput collection and write it to the Elixir port
		private void SendUserInput()
		{
			Statics.UserInput.TryTake(out string strResult, 1);
			if (null != strResult)
			{
				port.Write(strResult);
			}
		}

		// Do the cleanup needed to dispose this class
		protected override void DoCleanup()
		{
			timer.Stop();
			port.Dispose();
		}
	}
}
