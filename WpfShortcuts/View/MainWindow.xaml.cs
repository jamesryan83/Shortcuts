using CommonUI.Behaviors;
using System.Windows;

namespace WpfShortcuts.View
{
	public partial class MainWindow : Window
	{
		WindowResizing windowResizing;
		WindowDragging windowDragging;

		public MainWindow()
		{
			InitializeComponent();

			Style = (Style) FindResource(typeof(Window));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			windowResizing = new WindowResizing(this);
			windowDragging = new WindowDragging(this, false);
		}

	}
}
