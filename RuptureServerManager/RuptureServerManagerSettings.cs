using System.Text.Json.Serialization;

namespace RuptureServerManagerSettingsNS
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
		public bool StartNewGame { get; set; } = false;

		/// <summary>
		/// Whether the server should load a previously saved game. Default is false.
		/// </summary>
		public bool LoadSavedGame { get; set; } = true;

		/// <summary>
		/// Name of the save file to use. Default is "AutoSave0.sav".
		/// </summary>
		public string SaveGameName { get; set; } = "AutoSave0.sav";

		/// <summary>
		/// TCP/UDP port on which the server will listen. The default is 7777.
		/// This value is persisted for the configuration file but will be omitted
		/// when generating the version stored in the serverfiles directory.
		/// </summary>
		public int Port { get; set; } = 7777;

		/// <summary>
		/// 
		public int UpdateEnabled { get; set; } = 0;

		/// <summary>
		/// 
		public int UpdateInterval { get; set; } = 30;
	}
}
