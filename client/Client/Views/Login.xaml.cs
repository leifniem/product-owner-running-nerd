using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LoadRunnerClient
{
    /// <summary>
    /// Author Florian Ortmann
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			Window main = Application.Current.MainWindow;
		}
	}
}