using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LoadRunnerClient
{
    /// <summary>
    /// Interaktionslogik für EditorWindow.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private EditorViewModel viewModel => DataContext as EditorViewModel;

        public Editor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update Own Cursor Position
        /// </summary>
        /// <param name="sender">EditorWindow</param>
        /// <param name="e">Event Arguments</param>
        private void HighlightGrid(object sender, MouseEventArgs e)
        {
            // Cursor Position Relative To Editiong Area
            Point pos = e.GetPosition(CursorOverlay);
            viewModel.HighlightGrid(pos);
        }

        public void SetActive(object sender, RoutedEventArgs e)
        {
            viewModel.SetActive(sender, e);
        }

        #region Notification

        public void OnEditorInfoReceived(string msg)
        {
            EditorInfoBox.Text = msg;
            Storyboard sb = Resources["showNotification"] as Storyboard;
            sb.Begin(Notification);
        }

        #endregion Notification

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			viewModel.handleSliderChange(e.NewValue);
        }

        private void CursorOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(CursorOverlay);
			if (e.RightButton == MouseButtonState.Pressed)
				viewModel.MouseDownRight(pos);
			else
				viewModel.MouseDownLeft(pos);
		}

        private void CursorOverlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
			viewModel.MouseUp();
        }

        private void CursorOverlay_MouseLeave(object sender, MouseEventArgs e)
        {
			viewModel.MouseLeave();
        }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditorBackmap.ItemsSource = viewModel.MapTiles;
			CursorOverlay.ItemsSource = viewModel.MapCursors;
			ItemBackmap.ItemsSource = viewModel.MapItems;
			EditorMinimap.ItemsSource = viewModel.MapTiles;
			EnemySpawnPointBackmap.ItemsSource = viewModel.EnemySpawns;

			Window main = Application.Current.MainWindow;
			main.SizeToContent = SizeToContent.WidthAndHeight;
			main.WindowState = WindowState.Normal;
			main.Width = this.Width;
			main.Height = this.Height;
		}	
	}
}