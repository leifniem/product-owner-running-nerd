using System.Collections.Generic;

namespace LoadRunnerClient.MapAndModel
{
	/// <summary>
	/// GameCharacters of a Session accessible by ID
	/// </summary>
    public class PlayerList : ObservableModelBase
    {
        private Dictionary<string, GameCharacter> _players = new Dictionary<string, GameCharacter>();

        public PlayerList()
        {
        }

        public void addPlayer(string id, GameCharacter player)
        {
            this._players.Add(id, player);
        }

        public void removePlayer(string id)
        {
            this._players.Remove(id);
        }

        public void updatePosition(string id, float x, float y)
        {
            this._players[id].PosX = x;
            this._players[id].PosY = y;
        }

        public ICollection<GameCharacter> getPlayers()
        {
            return _players.Values;
        }
    }
}