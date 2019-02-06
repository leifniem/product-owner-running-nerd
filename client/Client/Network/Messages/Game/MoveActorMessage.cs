using LoadRunnerClient.MapAndModel;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
	public class MoveActorMessage
    {
        public const string TYPE = "MoveActorMessage";

        public MoveActorMessage()
        {
        }

        /// <summary>
        /// MoveActorMessage Constructor
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="xy"></param>
        /// <param name="state"></param>
        public MoveActorMessage(string uuid, Vector2 xy, GameCharacterState state)
        {
            this._uuid = uuid;
            this._newPosition = xy;
            this._state = state;
        }

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private Vector2 _newPosition;

        public Vector2 newPosition
        {
            get => _newPosition;
            set => _newPosition = value;
        }

        private GameCharacterState _state;

        public GameCharacterState state
        {
            get => _state;
            set => _state = value;
        }
    }
}