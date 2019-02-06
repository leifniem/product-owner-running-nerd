using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Network.Messages
{
    public class ItemPickedUpMessage
    {
        public const string TYPE = "ItemPickedUpMessage";

        public ItemPickedUpMessage()
        {
        }

        /// <summary>
        /// ItemPickedUpMessage Constructor
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="itemDTO"></param>
        public ItemPickedUpMessage(string uuid, ItemDTO itemDTO)
        {
            this._uuid = uuid;
            this._itemDTO = itemDTO;
        }

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private ItemDTO _itemDTO;

        public ItemDTO itemDTO
        {
            get => _itemDTO;
            set => _itemDTO = value;
        }
    }
}