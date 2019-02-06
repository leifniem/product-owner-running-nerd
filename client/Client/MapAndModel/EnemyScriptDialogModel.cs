using LoadRunnerClient.DTOs;
using LoadRunnerClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.MapAndModel
{

	/// <summary>
	/// Model for creating and editing scripted enemy spawnpoints in the map editor.
	/// </summary>
	class EnemyScriptDialogModel : ObservableModelBase
	{
		private ClientChannelHandler _clientChannelHandler;
		public ClientChannelHandler ClientChannelHandler
		{
			get => _clientChannelHandler;
			set => _clientChannelHandler = value;
		}

		private int _posX = 0;
		private int _posY = 0;
		private bool _locked;

		private string _name;
		private string _code;

		/// <summary>
		/// The name of the enemy
		/// </summary>
		public string name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged("name");
			}
		}

		/// <summary>
		/// The Python code that this enemy will run.
		/// </summary>
		public string code
		{
			get => _code;
			set
			{
				_code = value;
				OnPropertyChanged("code");
			}
		}

		/// <summary>
		/// If an existing enemy spawn point is edited It should be locked, 
		/// so no other User can access it, until editing is finished.
		/// </summary>
		public bool locked
		{
			get => this._locked;
			set => this._locked = value;
		}

		/// <summary>
		/// X-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int posX
		{
			get => this._posX;
			set => this._posX = value;
		}

		/// <summary>
		/// Y-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int posY
		{
			get => this._posY;
			set => this._posY = value;
		}

		/// <summary>
		/// Instantiates the model and gets a reference to the ClientChannelHandler.
		/// </summary>
		public EnemyScriptDialogModel()
		{
			ClientChannelHandler = ClientChannelHandler.getInstance();			
		}
	
		/// <summary>
		/// Confirm editing and send a message with the current spawn point data to the server.
		/// </summary>
		public void Confirm()
		{
			var dto = new EnemySpawnPointDTO() { name = this.name, code = this.code, gridX = this._posX, gridY = this._posY };
			this.ClientChannelHandler.SendEnemySpawnPointMessage(dto);
			this.locked = false;
		}

		/// <summary>
		/// Cancel editing. If an existing spawn point was being edited, 
		/// it will be unlocked for other users again.
		/// </summary>
		public void Close()
		{
			if (this.locked)
				this.ClientChannelHandler.SendEnemySpawnPointUnlockMessage(this._posX, this._posY);
		}

	}
}
