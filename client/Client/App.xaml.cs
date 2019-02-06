using LoadRunnerClient.Network;
using System.Windows;

namespace LoadRunnerClient
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);
            MainWindow mw = new MainWindow
            {
                DataContext = new MainViewModel(),
				Title = "Product Owner: Running Nerd"
            };
            mw.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            NetworkService.Disconnect();
        }
    }
}