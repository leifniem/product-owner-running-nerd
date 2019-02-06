using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Network.Messages {
	class EnemySpawnPointUnlockMessage {

		public const string TYPE = "EnemySpawnPointUnlockMessage";

		private int _gridX, _gridY;

		public int gridX {
			get => _gridX;
			set => _gridX = value;
		}

		public int gridY {
			get => _gridY;
			set => _gridY = value;
		}

		public EnemySpawnPointUnlockMessage(int gridX, int gridY) {
			this.gridX = gridX;
			this.gridY = gridY;		
		}

		public EnemySpawnPointUnlockMessage() {
		}
	}
}
