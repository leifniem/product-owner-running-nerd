using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LoadRunnerClient {
	/// <summary>
	/// ViewModel of the EnemyScriptDialog. 
	/// Allows a user to create or edit scripted enemy spawn points in the map editor.
	/// </summary>
	internal class EnemyScriptDialogViewModel : ObservableViewModelBase {
		EnemyScriptDialogModel _model;

		/// <summary>
		/// The name of the enemy
		/// </summary>
		public string name {
			get => this._model.name;
			set => this._model.name = value;
		}

		/// <summary>
		/// The Python code that this enemy will run.
		/// </summary>
		public string code {
			get => this._model.code;
			set => this._model.code = value;
		}

		/// <summary>
		/// If an existing enemy spawn point is edited It should be locked, 
		/// so no other User can access it, until editing is finished.
		/// </summary>
		public bool locked {
			get => this._model.locked;
			set => this._model.locked = value;
		}

		/// <summary>
		/// X-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int posX {
			get => this._model.posX;
			set => this._model.posX = value;
		}

		/// <summary>
		/// Y-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int posY {
			get => this._model.posY;
			set => this._model.posY = value;
		}

		/// <summary>
		/// Confirm command to apply changes, send them to the server 
		/// and close the dialog window.
		/// </summary>
		private ICommand _confirmCommand;
		public ICommand ConfirmCommand {
			get {
				if (_confirmCommand == null) {
					_confirmCommand = new ActionCommand(e => Confirm());
				}
				return _confirmCommand;
			}
		}

		/// <summary>
		/// Cancel command to discard changes, unlock the spawn point for other users 
		/// and close the dialog window.
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
		/// Instantiates the view model and creates a model object.
		/// </summary>
		public EnemyScriptDialogViewModel() {
			this._model = new EnemyScriptDialogModel();
		}

		/// <summary>
		/// Confirm editing and send a message with the current spawn point data to the server.
		/// </summary>
		public void Confirm() {
			_model.Confirm();
			CloseAction();
		}

		/// <summary>
		/// Cancel editing. If an existing spawn point was being edited, 
		/// it will be unlocked for other users again.
		/// </summary>
		public void Cancel() {
			_model.Close();
			CloseAction();
		}

		public void Close() {
			_model.Close();
		}

	}
}