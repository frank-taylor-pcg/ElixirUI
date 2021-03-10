using System;
using System.IO;
using System.Text;

namespace ElixirUI
{
	public class Port : DisposableBase
	{
		private const int iBufferSize = 2048;

		private readonly Stream stdin;
		private readonly Stream stdout;

		/// <summary>
		/// Constructor. Opens the streams needed by the port.
		/// </summary>
		public Port()
		{
			stdin = Console.OpenStandardInput();
			stdout = Console.OpenStandardOutput();
		}

		/// <summary>
		/// Reads in messages from the connected Elixir port
		/// </summary>
		/// <returns>The message received as a UTF8 string</returns>
		public string Read()
		{
			if (false == stdin.CanRead) return null;

			StringBuilder sb = new();

			byte[] buffer = new byte[iBufferSize];
			int iNumBytes = stdin.Read(buffer, 0, buffer.Length);

			// TODO: Investigate using a while loop to read the entire incoming message even if it exceeds the buffer size
			if (iNumBytes > 0)
			{
				sb.Append(Encoding.UTF8.GetString(buffer, 0, iNumBytes));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Sends some text to the connected Elixir port
		/// </summary>
		/// <param name="strOutput">The message to send (gets converted to UTF8)</param>
		/// <returns>A debug string showing the length, buffer, decoded buffer and time to send</returns>
		public string Write(string strOutput)
		{
			DateTime start = DateTime.Now;
			byte[] bOutput = Encoding.UTF8.GetBytes(strOutput);
			stdout.Write(bOutput);
			int iDuration = (int)(DateTime.Now - start).TotalMilliseconds;

			// Return some debug information
			return $"{bOutput.Length} | {bOutput} | {Encoding.UTF8.GetString(bOutput, 0, bOutput.Length)} |  Time to send: {iDuration}";
		}

		// Do the cleanup needed to dispose this class
		protected override void DoCleanup()
		{
			stdin.Dispose();
			stdout.Dispose();
		}
	}
}
