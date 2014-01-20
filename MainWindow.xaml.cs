using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace AVR_Simulator
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			ConsoleManager.Show();

			this.Worker = new BackgroundWorker()
			{
				WorkerSupportsCancellation = true
			};
			this.Worker.DoWork += new DoWorkEventHandler(this.Worker_DoWork);

			InitializeComponent();
		}

		private BackgroundWorker Worker;
		private Atmega328Interpreter Interpreter;

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Worker.RunWorkerAsync();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			this.Worker.CancelAsync();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			this.Interpreter = new Atmega328Interpreter();

			/*this.Interpreter.Load(
				IntelHEX.Parse(@"C:\Users\Tom\Documents\Atmel Studio\Projects\Blink\Debug\Blink.hex"),
				IntelHEX.Parse(@"C:\Users\Tom\Documents\Atmel Studio\Projects\Blink\Debug\Blink.eep")
				);*/

			this.Interpreter.Load(IntelHEX.Parse("Blink.hex"));

			this.Interpreter.PORTB.PB5.ValueChanged += new EventHandler<AVRInterpreter.GPIOPinValueChangedEventArgs>(this.PB5_ValueChanged);

			for (; !this.Worker.CancellationPending; )
			{
				this.Interpreter.Execute();
			}
		}

		private void PB5_ValueChanged(object sender, AVRInterpreter.GPIOPinValueChangedEventArgs e)
		{
			this.Dispatcher.Invoke(new Action(() =>
			{
				this.Background = e.NewValue ? Brushes.Red : Brushes.Black;
			}));
		}
	}
}
