using System.Windows;
using System.Windows.Controls;

namespace LoadRunnerClient
{
    /// <summary>
    /// Author Florian Ortmann
    /// Interaktionslogik für GameEnding.xaml
    /// </summary>
    public partial class GameEnding : UserControl
    {
        public GameEnding()
        {
            InitializeComponent();		
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {			
			Window main = Application.Current.MainWindow;
			main.SizeToContent = SizeToContent.WidthAndHeight;
			main.WindowState = WindowState.Normal;
			main.Width = this.Width;
			main.Height = this.Height;
		}

	}
}
