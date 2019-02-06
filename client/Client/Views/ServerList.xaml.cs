using System;
using System.Windows;
using System.Windows.Controls;
using LoadRunnerClient.Util;

namespace LoadRunnerClient
{
    /// <summary>
    /// Interaktionslogik für ServerList.xaml
    /// </summary>
    public partial class ServerList : UserControl
    {

		private ServerListViewModel viewModel { get => DataContext as ServerListViewModel; }
        public ServerList()
        {
            InitializeComponent();
        }

		private void DenyEvent(object sender, ErrorMessageEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(e.message);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel.AddDenyEvent(DenyEvent);
			Window main = Application.Current.MainWindow;
		}

		public void AboutToggle(object sender, RoutedEventArgs e)
		{
			viewModel.ShowAbout = !viewModel.ShowAbout;
		}
	}
}