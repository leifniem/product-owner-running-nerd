using LoadRunnerClient.DTOs;
using System.Collections.Generic;

namespace LoadRunnerClient.Messages
{
    /// <summary>
    /// Autor: Florian Ortmann
    /// </summary>
    public class MapListMessage
    {
        public const string TYPE = "MapListMessage";
        private List<MapMetaDTO> _listOfMaps;

        public MapListMessage()
        {
        }

        public MapListMessage(List<MapMetaDTO> maps)
        {
            this._listOfMaps = maps;
        }

        public List<MapMetaDTO> listOfMaps
        {
            get => _listOfMaps;
            set => _listOfMaps = value;
        }
    }
}