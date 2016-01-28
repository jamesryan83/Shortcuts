
using Common;
using CommonUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfShortcuts.Model;
using WpfShortcuts.Properties;

namespace WpfShortcuts.ViewModel
{
	class MainWindowContentViewModel : ViewModelBase
	{

		public ObservableCollection<ShortcutItem> items { get; set; }


		#region Properties & Commands

		private string _currentFileName;
		public string currentFileName
		{
			get { return _currentFileName; }
			set { _currentFileName = value; OnPropertyChanged("currentFileName"); }
		}

		private bool _showSaveText;
		public bool showSaveText
		{
			get { return _showSaveText; }
			set { _showSaveText = value; OnPropertyChanged("showSaveText"); }
		}

		private string _filterText;
		public string filterText
		{
			get { return _filterText; }
			set
			{
				_filterText = value;
				OnPropertyChanged("filterText");
				itemCollectionView.Refresh();
			}
        }

		private ICollectionView _itemCollectionView;
		public ICollectionView itemCollectionView
		{
			get { return _itemCollectionView; }
			set { _itemCollectionView = value; OnPropertyChanged("itemCollectionView"); }
		}


		private ShortcutItem _selectedItem;
		public ShortcutItem selectedItem
		{
			get { return _selectedItem; }
			set { _selectedItem = value; OnPropertyChanged("selectedItem"); }
		}


		// Commands
		public ICommand saveItemsCommand { get; set; }
		public ICommand saveItemsAsCommand { get; set; }
		public ICommand importItemsCommand { get; set; }		
		public ICommand copyItemPathCommand { get; set; }
		public ICommand removeItemCommand { get; set; }
		public ICommand clearListCommand { get; set; }

		#endregion



		// Constructor
		public MainWindowContentViewModel()
		{
			showSaveText = false;

			// Collections and filter
            items = new ObservableCollection<ShortcutItem>();
			itemCollectionView = CollectionViewSource.GetDefaultView(items);
			itemCollectionView.Filter = listBoxFilterItems;

			// Events
			EventSystem.RegisterCallbackOnEvent("dataDropped", dataDropped);
			EventSystem.RegisterCallbackOnEvent("dragOver", dragOver);

			// Commands
			saveItemsCommand = new RelayCommand(x => saveItems());
			saveItemsAsCommand = new RelayCommand(x => currentFileName = MainData.GetInstance().SaveAsItems(items));			
			copyItemPathCommand = new RelayCommand(x => copyPathToClipboard());
			removeItemCommand = new RelayCommand(x => removeItemFromList());
			clearListCommand = new RelayCommand(x => items.Clear());
			importItemsCommand = new RelayCommand(x =>
			{
				string filePath;
				addItemsToList(MainData.GetInstance().LoadShortcutItemsWithDialog(out filePath));
				currentFileName = filePath;
            });

			// Load previously open items
			if (File.Exists(Settings.Default.lastOpenedList) == true)
			{
				currentFileName = Settings.Default.lastOpenedList;
				addItemsToList(MainData.GetInstance().LoadShortcutItemsWithoutDialog(Settings.Default.lastOpenedList));
			}
		}


		// Add items to the listbox
		private void addItemsToList(ObservableCollection<ShortcutItem> tempItems)
		{
			if (tempItems == null)
				return;

			items.Clear();
			foreach (ShortcutItem s in tempItems)
				items.Add(s);			
		}


		// Save Items to last used filepath
		private void saveItems()
		{
			// save without dialog
			if (File.Exists(Settings.Default.lastOpenedList) == true)
				MainData.GetInstance().SaveItems(items, Settings.Default.lastOpenedList);

			// save with dialog if not saved before
			else
				currentFileName = MainData.GetInstance().SaveAsItems(items);

			showSaveText = true;
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += delegate { Thread.Sleep(1000); };
			bw.RunWorkerCompleted += delegate { showSaveText = false; };
			bw.RunWorkerAsync();
        }


		// Copy the path of the selected item to the clipboard
		private void copyPathToClipboard()
		{
			if (selectedItem != null)
				TextUtil.SaveTextToWindowsClipboard(selectedItem.path);
        }



		// Remove the selected item from the list
		private void removeItemFromList()
		{
			if (selectedItem != null)
				items.Remove(selectedItem);
        }



		// Text Filter for a ListBoxSnippetsItem
		private bool listBoxFilterItems(object item)
		{			
			if (string.IsNullOrEmpty(filterText))
				return true;
			else
				return ((item as ShortcutItem).displayName.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);
		}



		#region Drag/Drop

		// Item Dropped onto dropzone
		public void dataDropped(object sender, EventSystemEventArgs e)
		{
			DragEventArgs args = e.data as DragEventArgs;
			if (args != null)
			{
				// Put paths into array
				string[] temp = null;
				if (args.Data.GetDataPresent(DataFormats.FileDrop))
					temp = ((string[]) args.Data.GetData(DataFormats.FileDrop));

				foreach (string s in temp)
				{
					// is directory
					if (FileFolderUtil.IsPathADirectory(s) == true)
					{
						items.Add(new ShortcutItem
						{
							displayName = FileFolderUtil.GetDirectoryName(s),
							path = s,
							isFile = false,
							image = ImageUtil.LoadBitmapImageFromResources("Resources/folder_32.png")
						});
					}
					// is file
					else if (File.Exists(s) == true)
					{						
						items.Add(new ShortcutItem
						{
							displayName = Path.GetFileNameWithoutExtension(s),
							path = s,
							isFile = true,
							image = MainData.GetInstance().GetFileImage(s)
						});
					}
				}
			}
		}


		// Item dragged over dropzone
		public void dragOver(object sender, EventSystemEventArgs e)
		{
			DragEventArgs args = e.data as DragEventArgs;
			if (args != null)
			{
				args.Handled = true;
				args.Effects = DragDropEffects.Copy;
			}
		}

		#endregion


	}


	
}
