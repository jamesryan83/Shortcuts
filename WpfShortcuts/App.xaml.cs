using Common;
using CommonUI.Views;
using System;
using System.Windows;

namespace WpfShortcuts
{
	public partial class App : Application
	{
		// Constructor
		public App()
			: base()
		{
			this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		// Unhandled Exception Handler
		void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			ExtendedArgumentException ex = e.Exception as ExtendedArgumentException;
			if (ex != null)
			{
				// Show error dialog
				ErrorDialog dialog = new ErrorDialog(ex);
				dialog.Show();

				DataUtil.WriteCrashToErrorLogsFolder<ExtendedArgumentException>(ex, ex.message);
			}
			else
				DataUtil.WriteCrashToErrorLogsFolder<Exception>(e.Exception, "Unhandled Exception");

			e.Handled = true;
		}
	}
}
