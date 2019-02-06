using System;
using System.Windows;
using System.Windows.Controls;
using LoadRunnerClient.MapAndModel.ViewModel;
using LoadRunnerClient.Util;

namespace LoadRunnerClient
{
    /// <summary>
    /// Autor Florian Ortmann
    ///
    /// Interaktionslogik für CreateSession.xaml
    /// </summary>
    public partial class CreateSession : UserControl
    {
        private CreateSessionViewModel viewModel { get => DataContext as CreateSessionViewModel; }
        
        public CreateSession()
        {
            InitializeComponent();
        }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel.AddDenyEvent(OnDeny);

			Window main = Application.Current.MainWindow;
		}

		private void OnDeny(object sender, ErrorMessageEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(e.message);
		}

		private void SessionChecked(object sender, RoutedEventArgs e)
        {
            if (MapName == null || EditMap == null || NewMap == null)
            {
                return;
            }
            if (EditorSession.IsChecked.Value && NewMap.IsChecked.Value)
            {
                MapName.Visibility = Visibility.Visible;
                MapBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                MapName.Visibility = Visibility.Collapsed;
                MapBox.Visibility = Visibility.Visible;
            }
            if (GameSession.IsChecked.Value)
            {
                MapBox.Visibility = Visibility.Visible;
            }
        }

	}
}