using LoadRunnerClient.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LoadRunnerClient
{
    /// <summary>
    /// Author Leif & Florian
    /// Game UserControl to show the running game
    /// </summary>
    public partial class Game : UserControl
    {
        public Game()
        {			
			main = Application.Current.MainWindow;		
            InitializeComponent();		
		}

        private GameViewModel ViewModel { get => DataContext as GameViewModel; }
		public Window main { get; private set; }
		private double originalWidth, originalHeight;

		MediaPlayer backgroundMusic = new MediaPlayer();

		/// <summary>
		/// Method which is called when a Key is pressed 
		/// Sends the Key to the ViewModel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">KeyEvent which will be send to the ViewModel</param>
		private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {		
			if (ViewModel == null)			
				return;			

			if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Return)) {
				ToggleFullscreen();
				return;
			}

			if (e.Key == Key.Escape) {
				var dialog = new QuitGameDialog();
				dialog.ShowDialog();
				return;
			}

			ViewModel.HandleKeyDown(e);
        }

		private void ToggleFullscreen() {			
			SetFullScreen(main.WindowState != WindowState.Maximized);			
		}

		public void SetFullScreen(Boolean fullscreen) {
			main.SizeToContent = SizeToContent.Manual;
			main.WindowStyle = fullscreen ? WindowStyle.None : WindowStyle.SingleBorderWindow;
			main.WindowState = fullscreen ? WindowState.Maximized : WindowState.Normal;
		}

		/// <summary>
		/// Method which is called when a key is released
		/// Sends the key to the ViewModel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">KeyEvent which will be send to the ViewModel</param>
		private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (ViewModel == null) return;
            ViewModel.HandleKeyUp(e);
        }

        /// <summary>
        /// Method which will be called when the UserControl is unloaded
        /// And calls the ViewModel unloaded method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnloadedEventHandler(object sender, RoutedEventArgs e)
        {
			backgroundMusic.Stop();
			backgroundMusic.MediaEnded -= resetBackground;
            var window = Window.GetWindow(this);
			main.KeyDown -= OnKeyDownHandler;
			main.KeyUp -= OnKeyUpHandler;
			SetFullScreen(false);
			if (ViewModel == null) return;
            ViewModel.UnLoaded();
        }

        /// <summary>
        /// Method which is called when the UserControl is loaded
        /// Initializes the Itemsources of the view and registers the KeyEventHandler to the MainWindow where the UserControl is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadedEventHandler(object sender, RoutedEventArgs e)
        {
			backgroundMusic.Open(new Uri("./Resources/Sounds/loop.mp3", UriKind.Relative));
			backgroundMusic.MediaEnded += resetBackground;
			var window = Window.GetWindow(this);
            window.KeyDown += OnKeyDownHandler;
            window.KeyUp += OnKeyUpHandler;
			GameBackmap.ItemsSource = ViewModel.RelevantSolids;
			ItemBackmap.ItemsSource = ViewModel.RelevantItems;
            PlayerVisuals.ItemsSource = ViewModel.Players;
			HealthAndInventory.ItemsSource = ViewModel.Players;
			SetFullScreen(true);
			backgroundMusic.Play();
			//	Width = "{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type UserControl}},Path=ActualWidth}"
			//	Height = "{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type UserControl}},Path=ActualHeight}"
			//	Binding bind = new Binding
			//	{
			//		RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1),
			//		Path = new PropertyPath(UserControl.ActualWidthProperty)
			//	};
			//	GameContainer.SetBinding(WidthProperty, bind);

		}

		private void resetBackground(object sender, EventArgs e)
		{
			backgroundMusic.Stop();
			backgroundMusic.Position = TimeSpan.Zero;
			backgroundMusic.Play();
		}
	}

	public class WidthBindingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int widthChange = int.Parse(parameter as string);
			double input = (double)value;
			return input + widthChange;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}