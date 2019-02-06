using LoadRunnerClient.DTOs;
using LoadRunnerClient.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LoadRunnerClient
{

    /// <summary>
    /// Interaction logic for NewEnemyDialog.xaml
    /// </summary>
    public partial class EnemyScriptDialog : Window
    {
		EnemyScriptDialogViewModel viewModel;

		public EnemyScriptDialog(string name, string code, bool locked, int gridX, int gridY)
        {
            InitializeComponent();

			viewModel = new EnemyScriptDialogViewModel();
			viewModel.name = name;
			viewModel.code = code;
			viewModel.locked = locked;
			viewModel.posX = gridX;
			viewModel.posY = gridY;
			viewModel.CloseAction = new Action(this.Close);

			DataContext = viewModel;

		}

		private void OnClose(object sender, CancelEventArgs e) {
			viewModel.Close();
		}

	}
}
