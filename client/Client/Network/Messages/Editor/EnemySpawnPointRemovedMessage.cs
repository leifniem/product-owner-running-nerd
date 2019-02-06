using LoadRunnerClient.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Network.Messages {
	class EnemySpawnPointRemovedMessage {

		public const string TYPE = "EnemySpawnPointRemovedMessage";

		public EnemySpawnPointRemovedMessage() { }

		public EnemySpawnPointRemovedMessage(EnemySpawnPointDTO enemySpawnDTO) {
			this.enemySpawnDTO = enemySpawnDTO;
		}

		private EnemySpawnPointDTO _enemySpawnDTO;
		public EnemySpawnPointDTO enemySpawnDTO {
			get => _enemySpawnDTO;
			set => _enemySpawnDTO = value;
		}

	}
}
