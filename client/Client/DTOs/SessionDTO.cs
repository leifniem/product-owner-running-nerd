namespace LoadRunnerClient.DTOs
{
    /// <summary>
	/// DTO for transmitting information about an Editor- or GameSession
	/// </summary>
    public class SessionDTO
    {
        private string _id;
        private string _name;
        private int _users;
        private int _ping;
        private bool _editingSession;
        private bool _gameSession;
        private MapMetaDTO _mapMetaDTO;
        private int _minUser;

        public string id { get => _id; set => _id = value; }
        public string name { get => _name; set => _name = value; }
        public int users { get => _users; set => _users = value; }
        public int minUser { get => _minUser; set => _minUser = value; }
        public int ping { get => _ping; set => _ping = value; }
        public bool editorSession { get => _editingSession; set => _editingSession = value; }
        public bool gameSession { get => _gameSession; set => _gameSession = value; }
        public MapMetaDTO mapMetaDTO { get => _mapMetaDTO; set => _mapMetaDTO = value; }

        public SessionDTO()
        {
        }

		/// <summary>
		/// Constructor for SessionDTO with all parameters
		/// </summary>
		/// <param name="id">ID of Session</param>
		/// <param name="name">Name of Session</param>
		/// <param name="users">Number of Users in Session</param>
		/// <param name="editorSession">Boolean determining if this is an EditorSession</param>
		/// <param name="gameSession">Boolean determining if this is a GameSession</param>
        public SessionDTO(string id, string name, int users, bool editorSession, bool gameSession)
        {
            this.id = id;
            this.name = name;
            this.users = users;
            this.editorSession = editorSession;
            this.gameSession = gameSession;
        }
    }
}