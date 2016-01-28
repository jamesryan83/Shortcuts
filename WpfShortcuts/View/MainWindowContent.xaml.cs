using Common;
using CommonUI;
using System.Windows;
using System.Windows.Controls;

namespace WpfShortcuts.View
{
	public partial class MainWindowContent : UserControl
	{
		public event EventSystemEventHandler dropHandler;
		public event EventSystemEventHandler dragOverHandler;

		public MainWindowContent()
		{
			InitializeComponent();

			Style = (Style) FindResource(typeof(UserControl));

			EventSystem.CreateEvent("dataDropped", dropHandler);
			EventSystem.CreateEvent("dragOver", dragOverHandler);
		}


		#region Drag/Drop Events

		private void Listbox1_DragOver(object sender, DragEventArgs e)
		{
			EventSystem.RaiseEvent("dragOver", this, new EventSystemEventArgs(e));
		}

		private void GridDrop_PreviewDragEnter(object sender, DragEventArgs e)
		{
			ImageDragDrop.Source = ImageUtil.LoadBitmapImageFromResources("Resources/plus_hover.png");
		}

		private void GridDrop_PreviewDragLeave(object sender, DragEventArgs e)
		{
			ImageDragDrop.Source = ImageUtil.LoadBitmapImageFromResources("Resources/plus_normal.png");
		}

		private void Listbox1_Drop(object sender, DragEventArgs e)
		{			
            EventSystem.RaiseEvent("dataDropped", this, new EventSystemEventArgs(e));
			ImageDragDrop.Source = ImageUtil.LoadBitmapImageFromResources("Resources/plus_normal.png");
		}

		#endregion
		
	}
}
