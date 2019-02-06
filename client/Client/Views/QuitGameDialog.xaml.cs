using LoadRunnerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoadRunnerClient.Views {
	/// <summary>
	/// Interaction logic for QuitGameDialog.xaml
	/// </summary>
	public partial class QuitGameDialog : Window {

		private QuitGameDialogViewModel viewModel;

		public QuitGameDialog() {
			InitializeComponent();
			viewModel = new QuitGameDialogViewModel();
			viewModel.CloseAction = new Action(this.Close);
			DataContext = viewModel;
		}
	}
}
