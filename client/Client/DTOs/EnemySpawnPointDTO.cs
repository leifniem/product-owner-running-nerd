namespace LoadRunnerClient.DTOs {

	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about a scripted enemy and its spawn point.
	/// </summary>
	public class EnemySpawnPointDTO {

		private string _name;
		private string _code;
		private int _gridX, _gridY;

		/// <summary>
		/// Constructor of a EnemySpawnPointDTO
		/// </summary>
		/// <param name="name">The name of the enemy</param>
		/// <param name="code">The Python code that the enemy will run</param>
		/// <param name="gridX">X-Position of the enemy spawn point on the maps grid</param>
		/// <param name="gridY">Y-Position of the enemy spawn point on the maps grid</param>
		public EnemySpawnPointDTO(string name, string code, int gridX, int gridY) {
			this._name = name;
			this._code = code;
			this._gridX = gridX;
			this._gridY = gridY;
		}
		public EnemySpawnPointDTO() {
		}

		/// <summary>
		/// The name of the enemy
		/// </summary>
		public string name {
			get => _name;
			set => _name = value;
		}

		/// <summary>
		/// The Python code that the enemy will run
		/// </summary>
		public string code {
			get => _code;
			set => _code = value;
		}

		/// <summary>
		/// X-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int gridX {
			get => _gridX;
			set => _gridX = value;
		}

		/// <summary>
		/// Y-Position of the enemy spawn point on the maps grid
		/// </summary>
		public int gridY {
			get => _gridY;
			set => _gridY = value;
		}

	}
}