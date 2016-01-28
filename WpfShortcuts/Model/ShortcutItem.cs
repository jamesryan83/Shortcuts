using Common;
using CommonUI;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfShortcuts.Model
{
	// A single listbox item
	public class ShortcutItem : INotifyPropertyChanged
	{
		[JsonIgnore]
		public ICommand openFolderCommand { get; set; }

		[JsonIgnore]
		public ICommand startProgramCommand { get; set; }
				
		private ImageSource _image;
		[JsonIgnore]
		public ImageSource image
		{
			get { return _image; }
			set { _image = value; OnPropertyChanged("image"); }
		}

		private string _displayName;
		public string displayName
		{
			get { return _displayName; }
			set { _displayName = value; OnPropertyChanged("displayName"); }
		}

		private string _path;
		public string path
		{
			get { return _path; }
			set { _path = value; OnPropertyChanged("path"); }
		}

		private bool _isFile;
		public bool isFile
		{
			get { return _isFile; }
			set { _isFile = value; OnPropertyChanged("isFile"); }
		}


		public ShortcutItem()
		{
			openFolderCommand = new RelayCommand(x => buttonClicked(false));
			startProgramCommand = new RelayCommand(x => buttonClicked(true));
		}


		private void buttonClicked(bool start)
		{
			if (start == true)
				SystemUtil.RunFile(path);  // run will open a file or folder
			else
			{
				// open a folder or open a folder that a file is in
				if (FileFolderUtil.IsPathADirectory(path) == true)
					SystemUtil.RunFile(path);
				else				
					SystemUtil.RunFile(Path.GetDirectoryName(path));				
            }
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
