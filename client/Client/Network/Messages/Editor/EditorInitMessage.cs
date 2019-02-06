using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
    public class EditorInitMessage
    {
        public const string TYPE = "EditorInitMessage";

        private MapDTO _mapDTO;

        public EditorInitMessage(MapDTO mapDTO)
        {
            this._mapDTO = mapDTO;
        }

        public MapDTO mapDTO
        {
            get => _mapDTO;
            set => _mapDTO = value;
        }
    }
}