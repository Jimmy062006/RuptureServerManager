using System.Text.Json.Serialization;

namespace RuptureServerManager
{
    /// <summary>
    /// Represents the dedicated server settings that can be configured through the UI.
    /// These values are persisted to disk in JSON format.
    /// </summary>
    public class RuptureServerManagerSettings
	{
        /// <summary>
        /// Name of the session. Limited to 20 characters. Defaults to "DefaultSession".
        /// </summary>
        public string SessionName { get; set; } = "DefaultSession";

        /// <summary>
        /// Interval in seconds between save operations. Default is 300.
        /// </summary>
        public int SaveGameInterval { get; set; } = 300;

        /// <summary>
        /// Whether the server should start a new game. Default is true.
        /// </summary>
        public bool StartNewGame { get; set; } = true;

        /// <summary>
        /// Whether the server should load a previously saved game. Default is false.
        /// </summary>
        public bool LoadSavedGame { get; set; } = false;

        /// <summary>
        /// Name of the save file to use. Default is "AutoSave0.sav".
        /// </summary>
        public string SaveGameName { get; set; } = "AutoSave0.sav";

        /// <summary>
        /// TCP/UDP port on which the server will listen. This value is not persisted
        /// to the JSON file because it may be managed separately by the operating system
        /// or other configuration tools. The default is 7777.
        /// </summary>
        [JsonIgnore]
        public int Port { get; set; } = 7777;
    }
}