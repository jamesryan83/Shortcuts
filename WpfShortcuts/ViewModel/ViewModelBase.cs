using System.ComponentModel;

namespace WpfShortcuts.ViewModel
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		// Constructor
		public ViewModelBase()
		{

		}


		// Property Changed Event
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(name));
		}

	}
}
