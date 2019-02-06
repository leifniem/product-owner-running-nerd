using System.Windows;
using System.Windows.Controls;

namespace LoadRunnerClient
{
    /// <summary>
    /// Author Florian Ortmann
    /// Interaktionslogik für GameLobby.xaml
    /// </summary>
    public partial class GameLobby : UserControl
    {
        public GameLobby()
        {
            InitializeComponent();
        }

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			Window main = Application.Current.MainWindow;
		}
	}
}