using LoadRunnerClient.DTOs;
using System.Collections.Generic;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
	public class GameInitMessage
    {
        public const string TYPE = "GameInitMessage";

        private MapDTO _mapDTO;
        private int _minX;
        private int _maxX;
        private List<GameCharacterDTO> _playerDTOs;

        public GameInitMessage()
        {
        }

        public GameInitMessage(MapDTO mapDTO, int minX, int maxX, List<GameCharacterDTO> playerDTOs)
        {
            this._mapDTO = mapDTO;
            this._minX = minX;
            this._maxX = maxX;
            this._playerDTOs = playerDTOs;
        }

        public MapDTO mapDTO
        {
            get => _mapDTO;
            set => _mapDTO = value;
        }

        public int minX
        {
            get => _minX;
            set => _minX = value;
        }

        public int maxX
        {
            get => _maxX;
            set => _maxX = value;
        }

        public List<GameCharacterDTO> playerDTOs

        {
            get => _playerDTOs;
            set => _playerDTOs = value;
        }
    }
}