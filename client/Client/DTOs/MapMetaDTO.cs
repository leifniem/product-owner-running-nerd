namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about properties of a Map
	/// </summary>
	public class MapMetaDTO
    {
        private string _name;
        private int _numberOfSections;
        public int numberOfSections { get => _numberOfSections; set => _numberOfSections = value; }

        public MapMetaDTO()
        {
        }

		/// <summary>
		/// MapMetaDTO Constructor for name only transmission
		/// </summary>
		/// <param name="name">Name of the Map</param>
        public MapMetaDTO(string name)
        {
            this._name = name;
            this.numberOfSections = 1;
        }

		/// <summary>
		/// MapMetaDTO Constructor for providing Name and Number of Sections
		/// </summary>
		/// <param name="name">Name of the Map</param>
		/// <param name="numberOfSections">Number of Sections</param>
        public MapMetaDTO(string name, int numberOfSections)
        {
            this._name = name;
            this._numberOfSections = numberOfSections;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }
    }
}