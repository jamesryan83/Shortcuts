using Common;
using CommonJson;
using CommonUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfShortcuts.Properties;

namespace WpfShortcuts.Model
{

	// Main Data Class
	class MainData
	{
		
		#region Get Singleton Instance of MainData class

		private static MainData instance;

		// Constructor
		protected MainData()
		{			
		}

		// Get singleton instance
		public static MainData GetInstance()
		{
			if (instance == null)
				instance = new MainData();

			return instance;
		}

		#endregion


		// Load items with dialog
		public ObservableCollection<ShortcutItem> LoadShortcutItemsWithDialog(out string filePath)
		{			
			if (DialogUtil.ShowOpenFileDialog(out filePath, filter: "Json Files|*.json") == true)
			{
				return LoadShortcutItemsWithoutDialog(filePath);
            }
			else
				return null;
		}


		// TODO : error list
		// Load items
		public ObservableCollection<ShortcutItem> LoadShortcutItemsWithoutDialog(string filePath)
		{
			List<string> errorList = new List<string>();
			ObservableCollection<ShortcutItem> items = JsonUtil.LoadDataFromJsonFile<ObservableCollection<ShortcutItem>>(filePath);

			for (int i = 0; i < items.Count; i++)
			{
				// file
				if (items[i].isFile == true)
				{
					// check file exists
					if (File.Exists(items[i].path) == false)
					{
						errorList.Add("Missing File : " + items[i].path);
						items.Remove(items[i]);
						continue;
					}

					// get file image
					items[i].image = GetFileImage(items[i].path);
				}
				if (items[i].isFile == false)
				{
					// check folder exists
					if (FileFolderUtil.IsPathADirectory(items[i].path) == false)
					{
						errorList.Add("Missing Folder : " + items[i].path);
						items.Remove(items[i]);
						continue;
					}

					// get folder image
					items[i].image = ImageUtil.LoadBitmapImageFromResources("Resources/folder_32.png");
				}
			}


			// Save last opened list
			Settings.Default.lastOpenedList = filePath;
			Settings.Default.Save();


			return items;
		}


		// Save items As
		public string SaveAsItems(ObservableCollection<ShortcutItem> items)
		{
			string filePath = "";
			if (DialogUtil.ShowSaveFileDialog(out filePath, filter: "Json Files|*.json") == true)
			{				
				SaveItems(items, filePath);
				return filePath;
			}

			return "";
		}


		// Save items
		public void SaveItems(ObservableCollection<ShortcutItem> items, string filePath)
		{
			JsonUtil.SaveDataToJsonFile(items, filePath);
			Settings.Default.lastOpenedList = filePath;
			Settings.Default.Save();
		}


		// Returns a thumbnail of an image
		public ImageSource GetFileImage(string filePath)
		{
			string ext = Path.GetExtension(filePath);
			if (ext == ".jpg" || ext == ".png" || ext == ".bmp")
			{
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.DecodePixelWidth = 20;
				bi.CacheOption = BitmapCacheOption.OnLoad;
				bi.UriSource = new Uri(filePath);
				bi.EndInit();
				return bi;
			}
			else
				return ImageUtil.GetFileIcon(filePath);
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
