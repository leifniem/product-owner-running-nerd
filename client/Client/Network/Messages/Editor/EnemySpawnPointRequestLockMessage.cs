using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Network.Messages {
	class EnemySpawnPointRequestLockMessage {

		public const string TYPE = "EnemySpawnPointRequestLockMessage";

		private int _gridX, _gridY;

		public int gridX {
			get => _gridX;
			set => _gridX = value;
		}

		public int gridY {
			get => _gridY;
			set => _gridY = value;
		}

		public EnemySpawnPointRequestLockMessage(int gridX, int gridY) {
			this.gridX = gridX;
			this.gridY = gridY;		
		}

		public EnemySpawnPointRequestLockMessage() {
		}
	}
}
