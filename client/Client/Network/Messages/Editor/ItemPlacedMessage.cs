using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
    public class ItemPlacedMessage
    {
        public const string TYPE = "ItemPlacedMessage";
        
        public ItemPlacedMessage()
        {
        }

        public ItemPlacedMessage(ItemDTO itemDTO)
        {
            this.itemDTO = itemDTO;
        }

        private ItemDTO _itemDTO;

        public ItemDTO itemDTO
        {
            get => _itemDTO;
            set => _itemDTO = value;
        }    
    }
}
