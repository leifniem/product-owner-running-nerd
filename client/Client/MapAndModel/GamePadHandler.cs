using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using LoadRunnerClient.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.MapAndModel
{
	/// <summary>
	/// Handler class responsible for translating Gamepad input for GameModel and Messaging
	/// </summary>
	class GamePadHandler
	{

		private GameModel gameModel;
		private GameController _gameController;

		public GamePadHandler(GameModel gameModel){
			this.gameModel = gameModel;
			this._gameController = gameModel.GameController;
		}

		/// <summary>
		/// Handling of Controller Inputs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void handleControllerInput(object sender, PropertyChangedEventArgs e)
		{
			/// Action Buttons
			/// Jump
			if (e.PropertyName.Equals("A"))
			{
				if (_gameController.A && !gameModel.SpaceIsPressed)
				{
					gameModel.SpaceIsPressed = true;
					gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.JUMP);
					return;
				}
				else
				{
					gameModel.SpaceIsPressed = false;
					return;
				}
			}

			/// Energy
			else if (e.PropertyName.Equals("B"))
			{
				if (_gameController.B && !gameModel.EnergyIsPressed)
				{
					gameModel.EnergyIsPressed = true;
					gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.ENERGY);
					return;
				} else {
					gameModel.EnergyIsPressed = false;
					return;
				}
			}

			/// Dig Left
			else if (e.PropertyName.Equals("LeftShoulder"))
			{
				if (_gameController.LeftShoulder && !gameModel.DigLeftIsPressed)
				{
					gameModel.DigLeftIsPressed = true;
					gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DIG_LEFT);
					return;
				}
				else
				{
					gameModel.DigLeftIsPressed = false;
					return;
				}
			}

			/// Dig Left
			else if (e.PropertyName.Equals("RightShoulder"))
			{
				if (_gameController.RightShoulder && !gameModel.DigRightIsPressed)
				{
					gameModel.DigLeftIsPressed = true;
					gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DIG_RIGHT);
					return;
				}
				else
				{
					gameModel.DigRightIsPressed = false;
					return;
				}
			}

			/// Joystick X-Axis
			else if (e.PropertyName.Equals("XAxis"))
			{
				int x = _gameController.XAxis;

				if (x > -1 * GameController.STICK_LIMIT && x < GameController.STICK_LIMIT)
				{
					if (gameModel.RightIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.RIGHT); gameModel.RightIsPressed = false; }
					if (gameModel.LeftIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.LEFT); gameModel.LeftIsPressed = false; }
				}
				else if (x < -1 * GameController.STICK_LIMIT)
				{
					if (gameModel.RightIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.RIGHT); gameModel.RightIsPressed = false; }
					if (!gameModel.LeftIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.LEFT); gameModel.LeftIsPressed = true; }
				}
				else if (x > GameController.STICK_LIMIT)
				{
					if (!gameModel.RightIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.RIGHT); gameModel.RightIsPressed = true; }
					if (gameModel.LeftIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.LEFT); gameModel.LeftIsPressed = false; }
				}
			}

			/// Joystick Y-Axis
			else if (e.PropertyName.Equals("YAxis"))
			{
				int y = _gameController.YAxis;

				if (y > -1 * GameController.STICK_LIMIT && y < GameController.STICK_LIMIT)
				{
					if (gameModel.UpIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.UP); gameModel.UpIsPressed = false; }
					if (gameModel.DownIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DOWN); gameModel.DownIsPressed = false; }
				}
				else if (y < -1 * GameController.STICK_LIMIT)
				{
					if (gameModel.UpIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.UP); gameModel.UpIsPressed = false; }
					if (!gameModel.DownIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DOWN); gameModel.DownIsPressed = true; }
				}
				else if (y > GameController.STICK_LIMIT)
				{
					if (!gameModel.UpIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.UP); gameModel.UpIsPressed = true; }
					if (gameModel.DownIsPressed) { gameModel.ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DOWN); gameModel.DownIsPressed = false; }
				}
			}
		}
	}
}
