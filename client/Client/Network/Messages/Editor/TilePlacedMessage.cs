using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
    public class TilePlacedMessage
    {
        public const string TYPE = "TilePlacedMessage";

        public TilePlacedMessage()
        {
        }

        public TilePlacedMessage(TileDTO tileDTO)
        {
            this.tileDTO = tileDTO;
        }

        private TileDTO _tileDTO;

        public TileDTO tileDTO
        {
            get => _tileDTO;
            set => _tileDTO = value;
        }
    }
}