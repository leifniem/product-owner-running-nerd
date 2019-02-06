using LoadRunnerClient.MapAndModel;

namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about Characters in a Game Instance
	/// </summary>
	public class GameCharacterDTO
    {
        private string _uuid;
        private Vector2 _position;
        private string _color;
        private GameCharacterState _gameCharacterState;
        private bool _enemy;

        public GameCharacterDTO()
        {
        }

		/// <summary>
		/// Constructor of a Character DTO
		/// </summary>
		/// <param name="uuid">Server-provided uuid of GameCharacter</param>
		/// <param name="position">Vector 2 of Characters position</param>
		/// <param name="state">Current State of the Character</param>
        public GameCharacterDTO(string uuid, Vector2 position, GameCharacterState state)
        {
            this._uuid = uuid;
            this._gameCharacterState = state;
            this._position = position;
        }

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        public string color
        {
            get => _color;
            set => _color = value;
        }

        public Vector2 position
        {
            get => _position;
            set => _position = value;
        }

        public GameCharacterState gameCharacterState
        {
            get => _gameCharacterState;
            set => _gameCharacterState = value;
        }

        public bool enemy
        {
            get => _enemy;
            set => _enemy = value;
        }

		/// <summary>
		/// Method to Convert the DTO into an instance of GameCharacter
		/// </summary>
		/// <returns>GameCharacter instance</returns>
        public GameCharacter toGameCharacter()
        {
            GameCharacter character = new GameCharacter(this._uuid, this._color, this._enemy);
            character.PosX = (int)this._position.X;
            character.PosY = (int)this._position.Y;
            return character;
        }
    }
}