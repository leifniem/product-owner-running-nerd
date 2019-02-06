using LoadRunnerClient.MapAndModel;
using LoadRunnerClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LoadRunnerClient.ViewModels {

	/// <summary>
	/// ViewModel of the QuitGameDialog. 
	/// Allows the user to quit a running Game.
	/// </summary>
	class QuitGameDialogViewModel {

		ClientChannelHandler clientChannelHandler;

		public QuitGameDialogViewModel() {
			clientChannelHandler = ClientChannelHandler.getInstance();
		}

		/// <summary>
		/// Quit command to send a PlayerQuitMessage to the server 
		/// and close the dialog window.
		/// </summary>
		private ICommand _quitCommand;
		public ICommand QuitCommand {
			get {
				if (_quitCommand == null) {
					_quitCommand = new ActionCommand(e => Quit());
				}
				return _quitCommand;
			}
		}

		/// <summary>
		/// Cancel command to close the dialog window.
		/// </summary>
		private ICommand _canelCommand;
		public ICommand CancelCommand {
			get {
				if (_canelCommand == null) {
					_canelCommand = new ActionCommand(e => Cancel());
				}
				return _canelCommand;
			}
		}

		/// <summary>
		/// Event to trigger closing the dialog window.
		/// </summary>
		public Action CloseAction { get; set; }		

		/// <summary>
		/// Sends a PlayerQuitMessage to the server.
		/// </summary>
		public void Quit() {
			clientChannelHandler.sendPlayerQuitMessage();
			CloseAction();
		}

		/// <summary>
		/// Closes the dialog window.
		/// </summary>
		public void Cancel() {
			
			CloseAction();
		}

	}
}
