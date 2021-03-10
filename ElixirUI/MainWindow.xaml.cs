using System.Text;
using System.Threading;
using System.Windows;

namespace ElixirUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region DEPENDENCY PROPERTIES
		
		public string Input
		{
			get { return (string)GetValue(InputProperty); }
			set { SetValue(InputProperty, value); }
		}
		public static readonly DependencyProperty InputProperty =
				DependencyProperty.Register(nameof(Input), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

		public string Log
		{
			get { return (string)GetValue(LogProperty); }
			set { SetValue(LogProperty, value); }
		}
		public static readonly DependencyProperty LogProperty =
				DependencyProperty.Register(nameof(Log), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

		public int Cycles
		{
			get { return (int)GetValue(CyclesProperty); }
			set { SetValue(CyclesProperty, value); }
		}
		public static readonly DependencyProperty CyclesProperty =
				DependencyProperty.Register(nameof(Cycles), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

		#endregion DEPENDENCY PROPERTIES

		private readonly StringBuilder InputBuilder = new();
		private readonly StringBuilder LogBuilder = new();

		private bool bRunThread = true;
		private ThreadStart work;
		private Thread thread;

		private IOThread ioThread;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		// Gets server responses we care about, i.e., not the heartbeat that Elixir uses to update itself.
		private void GetMeaningfulServerResponse()
		{
			Statics.ServerResponses.TryTake(out string strServerResponse, 1);

			if (null != strServerResponse)
			{
				// I had to add a heartbeat to the Elixir code because I can't figure out how to get it to automatically update and see responses from the C#.
				if (false == strServerResponse.Equals("heartbeat"))
				{
					InputBuilder.AppendLine(strServerResponse.Trim());
				}
			}
		}

		private void HandleCommunication()
		{
			while (bRunThread)
			{
				Dispatcher.Invoke(() =>
				{
					Cycles += 1;

					GetMeaningfulServerResponse();

					Input = InputBuilder.ToString();
					//Log = LogBuilder.ToString();
				});
				Thread.Sleep(250);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			work = HandleCommunication;
			thread = new Thread(work);
			thread.Start();

			ioThread = new IOThread();
		}

		private void ExitApplication()
		{
			// Stop the running thread
			bRunThread = false;

			// Dispose of the IOThread object and shut down
			ioThread.Dispose();
			Application.Current.Shutdown();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ExitApplication();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			SubmitUserInput();
		}

		private void SubmitUserInput()
		{
			if (string.IsNullOrWhiteSpace(tbUserInput.Text)) return;

			Statics.UserInput.Add(tbUserInput.Text);
			LogBuilder.AppendLine($"Sending user input => {tbUserInput.Text}");
			Log = LogBuilder.ToString();
			tbUserInput.Text = string.Empty;
		}
	}
}
