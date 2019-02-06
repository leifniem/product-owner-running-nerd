using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Network.Messages
{
    class GameEndMessage
    {
        public const string TYPE = "GameEndMessage";
        public const string WIN_STATUS = "WIN";
        public const string LOSS_STATUS = "LOSS";
		public const string QUIT_STATUS = "QUIT";
		private Dictionary<string, int> _Scores;
		private string winnerColor;
		public Dictionary<string, int> Scores
        {
            get => _Scores;
            set => _Scores = value;
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => _status = value;
        }
		public string WinnerColor { get => winnerColor; set => winnerColor = value; }

		public GameEndMessage() { }

    }
}
