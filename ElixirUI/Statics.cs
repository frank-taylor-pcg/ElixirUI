using System.Collections.Concurrent;

namespace ElixirUI
{
	public static class Statics
	{

		private static BlockingCollection<string> _UserInput = new();
		public static BlockingCollection<string> UserInput { get => _UserInput; set => _UserInput = value; }


		private static BlockingCollection<string> _ServerResponses = new();
		public static BlockingCollection<string> ServerResponses { get => _ServerResponses; set => _ServerResponses = value; }
	}
}
