using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Messages {
    class EnemySpawnPointLockDenyMessage {

        public const string TYPE = "EnemySpawnPointLockDenyMessage";
       
        public EnemySpawnPointLockDenyMessage() { }

        public EnemySpawnPointLockDenyMessage(string reason) {
            _reason = reason;
        }

        private string _reason;
        public string reason {
            get => _reason;
            set => _reason = value;
        }
    }
}

