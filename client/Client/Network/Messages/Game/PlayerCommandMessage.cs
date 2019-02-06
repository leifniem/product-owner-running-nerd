using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
    public class PlayerCommandMessage
    {
        public const string TYPE = "PlayerCommandMessage";
        private PressedKey _pressedKey;

        public PlayerCommandMessage(PressedKey pressedKey)
        {
            this._pressedKey = pressedKey;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public PressedKey pressedKey
        {
            get => _pressedKey;
        }
    }

    public enum PressedKey
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        JUMP,
        DIG_LEFT,
        DIG_RIGHT,
        ENERGY
    }
}