using Common;
using CommonUI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfShortcuts.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{

		#region Properties and Variables

		// Commands
		public ICommand minimiseCommand { get; set; } // these commands never change so they can be auto properties
		public ICommand closeCommand { get; set; }

		private ICommand _restoreOrMaximiseCommand;
		public ICommand restoreOrMaximiseCommand  // this command is dynamically changed so it needs to call OnPropertyChanged
		{
			get { return _restoreOrMaximiseCommand; }
			set { _restoreOrMaximiseCommand = value; base.OnPropertyChanged("restoreOrMaximiseCommand"); }
		}

		// Path to current maximise button (it is also the restore button)
		private BitmapImage _restoreOrMaximiseImageButtonSource;
		public BitmapImage restoreOrMaximiseImageButtonSource
		{
			get { return _restoreOrMaximiseImageButtonSource; }
			set { _restoreOrMaximiseImageButtonSource = value; base.OnPropertyChanged("restoreOrMaximiseImageButtonSource"); }
		}

		// State of window.  Maximised, minimised etc.
		private WindowState _windowState;
		public WindowState windowState
		{
			get { return _windowState; }
			set
			{
				_windowState = value;

				// Set restoreOrMaximiseImageButtonSource and restoreOrMaximiseCommand depending on current window state
				if (value == WindowState.Maximized)
				{
					restoreOrMaximiseImageButtonSource = ImageUtil.LoadBitmapImageFromResources(@"../Resources/restore_normal.png");
					restoreOrMaximiseCommand = new RelayCommand(x => windowState = WindowState.Normal);
				}
				else
				{
					restoreOrMaximiseImageButtonSource = ImageUtil.LoadBitmapImageFromResources(@"../Resources/maximise_normal.png");
					restoreOrMaximiseCommand = new RelayCommand(x => windowState = WindowState.Maximized);
				}

				base.OnPropertyChanged("windowState");
			}
		}

		#endregion


		



		// Constructor
		public MainWindowViewModel()
		{
			// Default values
			windowState = WindowState.Normal;
			restoreOrMaximiseImageButtonSource = ImageUtil.LoadBitmapImageFromResources(@"../Resources/maximise_normal.png");

			// Title bar commands
			this.minimiseCommand = new RelayCommand(x => windowState = WindowState.Minimized);
			this.restoreOrMaximiseCommand = new RelayCommand(x => windowState = WindowState.Maximized);
			this.closeCommand = new RelayCommand(x => Application.Current.Shutdown());
		}

	}
}
