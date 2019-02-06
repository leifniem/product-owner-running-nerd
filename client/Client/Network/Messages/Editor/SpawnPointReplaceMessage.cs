using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Network.Messages
{
    public class SpawnPointReplaceMessage
    {
        public const string TYPE = "SpawnpointReplaceMessage";

        private Vector2Int _spawn;

        public SpawnPointReplaceMessage(int x, int y)
        {
            _spawn = new Vector2Int();
            _spawn.x = x;
            _spawn.y = y;
        }

        public Vector2Int spawn { get => _spawn; set => _spawn = value; }

    }

}
