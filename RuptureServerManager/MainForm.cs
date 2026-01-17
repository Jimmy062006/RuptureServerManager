using Microsoft.VisualBasic;
using RuptureServerManagerSettingsNS;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace RuptureServerManager
{
	/// <summary>
	/// Main form for the dedicated server controller. Handles UI events,
	/// persistence of settings, launching/updating external processes, and now
	/// supports an auto-update timer that monitors the configured Steam app ID
	/// for updates. When an update is detected (or on schedule), the server
	/// will be stopped, updated via SteamCMD, and then restarted.
	/// </summary>
	public partial class MainForm : Form
	{
		private RuptureServerManagerSettings _settings = new();
		private ServerManager? _serverManager;
		private ConfigManager<RuptureServerManagerSettings>? _configManager;
		private string _appFolder = string.Empty;
		private string _serverPath = string.Empty;
		private string _settingsFilePath = string.Empty;
		private string _steamCmdDir = string.Empty;
		private ServerUiState _uiState = ServerUiState.Idle;
		private readonly int _steamAppId = 3809400;
		private readonly string ServerSettingsFileName = "DSSettings.txt";
		private bool _isUpdating = false;
		private Timer? _autoUpdateTimer;
		private readonly TimeSpan _autoUpdateInterval = TimeSpan.FromMinutes(30);

		public MainForm()
		{
			InitializeComponent();
		}
	
		/// <summary>
		/// Loads persisted settings and ensures SteamCMD is available when the form loads.
		/// Initializes the auto-update timer.
		/// </summary>
		private async void MainForm_Load(object? sender, EventArgs e)
		{
			InitializePaths();
			_configManager = new ConfigManager<RuptureServerManagerSettings>(_settingsFilePath, AppendConsole);
			_settings = _configManager.Load();
			_serverManager = new ServerManager(_serverPath, AppendConsole);
			ApplySettingsToUi();
			await CheckAndDownloadSteamCMDAsync();
			StartAutoUpdateTimer();
		}

		/// <summary>
		/// Initializes the paths used by the application for storage and SteamCMD.
		/// Creates directories if they do not exist.
		/// </summary>
		private void InitializePaths()
		{
			// Base folder for application data located next to the executable
			_appFolder = Path.Combine(Application.StartupPath, "config");
			Directory.CreateDirectory(_appFolder);
			_settingsFilePath = Path.Combine(_appFolder, "RuptureServerManagerSettings.txt");
			_steamCmdDir = Path.Combine(Application.StartupPath, "steamcmd");
			Directory.CreateDirectory(_steamCmdDir);
			_serverPath = Path.Combine(Application.StartupPath, "serverfiles");
		}

		/// <summary>
		/// Loads settings from the JSON file if present. If the file cannot be parsed,
		/// defaults are used.
		/// </summary>
		private void LoadSettingsFromFile()
		{
			if (_configManager != null)
				_settings = _configManager.Load();
		}

		/// <summary>
		/// Updates the UI fields to reflect the currently loaded settings.
		/// </summary>
		private void ApplySettingsToUi()
		{
			// Ensure values are within allowed ranges
			portNumericUpDown.Value = Math.Clamp(_settings.Port, (int)portNumericUpDown.Minimum, (int)portNumericUpDown.Maximum);
			sessionNameTextBox.Text = _settings.SessionName;
			saveGameIntervalNumericUpDown.Value = Math.Clamp(_settings.SaveGameInterval, (int)saveGameIntervalNumericUpDown.Minimum, (int)saveGameIntervalNumericUpDown.Maximum);
			//startNewGameCheckBox.Checked = _settings.StartNewGame;
			//loadSavedGameCheckBox.Checked = _settings.LoadSavedGame;
			saveGameNameTextBox.Text = _settings.SaveGameName;
			autoUpdateCheckBox.Checked = _settings.UpdateEnabled == 1;
			updateIntervalTextBox.Text = _settings.UpdateInterval.ToString();
		}

		/// <summary>
		/// Writes the current settings object to disk as JSON. The port value is
		/// updated from the UI before saving.
		/// </summary>
		private void SaveSettingsToFile()
		{
			UpdateSettingsFromUi();
			if (_configManager != null)
				_configManager.Save(_settings);
			// Save server settings (excluding port)
			try
			{
				JsonSerializerOptions options = new() { WriteIndented = true };
				var serverSettings = new
				{
					_settings.SessionName,
					_settings.SaveGameInterval,
					_settings.StartNewGame,
					_settings.LoadSavedGame,
					_settings.SaveGameName,
				};
				var serverJson = JsonSerializer.Serialize(serverSettings, options: options);
				File.WriteAllText(Path.Combine(_serverPath, ServerSettingsFileName), serverJson);
			}
			catch (Exception ex)
			{
				AppendConsole($"Error saving server settings: {ex.Message}");
		}
		}

		/// <summary>
		/// Updates the internal settings object with values from UI controls.
		/// </summary>
		private void UpdateSettingsFromUi()
		{
			_settings.Port = (int)portNumericUpDown.Value;
			_settings.SessionName = sessionNameTextBox.Text.Trim();
			_settings.SaveGameInterval = (int)saveGameIntervalNumericUpDown.Value;
			_settings.StartNewGame = false;
			_settings.LoadSavedGame = true;
			_settings.SaveGameName = saveGameNameTextBox.Text.Trim();
			_settings.UpdateInterval = int.TryParse(updateIntervalTextBox.Text.Trim(), out int interval) ? interval : 30;
			_settings.UpdateEnabled = autoUpdateCheckBox.Checked ? 1 : 0;

			StartAutoUpdateTimer();
		}

		/// <summary>
		/// Disable controls when the server is starting so the user cannot
		/// modify settings or start the server again. Only the stop button
		/// remains enabled to allow for graceful shutdown.
		/// </summary>
		private void DisableButtonsOnStart()
		{
			saveSettingsButton.Enabled = false;
			//downloadSteamCmdButton.Enabled = false;
			startButton.Enabled = false;
			updateButton.Enabled = false;
			stopButton.Enabled = true;
		}

		private enum ServerUiState
		{
			Idle,
			ServerRunning,
			Busy
		}

		private void UpdateButtonStates()
		{
			if (InvokeRequired)
			{
				Invoke(UpdateButtonStates);
				return;
			}

			switch (_uiState)
			{
				case ServerUiState.Idle:
					saveSettingsButton.Enabled = true;
					startButton.Enabled = true;
					updateButton.Enabled = true;
					stopButton.Enabled = false;
					break;
				case ServerUiState.ServerRunning:
					saveSettingsButton.Enabled = true;
					startButton.Enabled = false;
					updateButton.Enabled = true;
					stopButton.Enabled = true;
					break;
				case ServerUiState.Busy:
					saveSettingsButton.Enabled = false;
					startButton.Enabled = false;
					updateButton.Enabled = false;
					stopButton.Enabled = _serverManager != null && _serverManager.IsRunning;
					break;
			}
		}


		private void SetButtonsEnabled(bool enabled)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<bool>(SetButtonsEnabled), enabled);
				return;
			}

			saveSettingsButton.Enabled = enabled;
			//downloadSteamCmdButton.Enabled = enabled;
			startButton.Enabled = enabled;
			updateButton.Enabled = enabled;
			stopButton.Enabled = enabled;
		}

		/// <summary>
		/// Sets whether the application is performing a long-running operation.
		/// Stop remains available if the server is running.
		/// Thread-safe.
		/// </summary>
		private void SetBusyState(bool busy)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<bool>(SetBusyState), busy);
				return;
			}

			_uiState = busy
				? ServerUiState.Busy
				: (_serverManager != null && _serverManager.IsRunning
					? ServerUiState.ServerRunning
					: ServerUiState.Idle);

			Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
			UpdateButtonStates();
		}


		/// <summary>
		/// Ensures that SteamCMD exists on disk. If the executable is not found,
		/// downloads and extracts the latest version from the official CDN.
		/// </summary>
		private async Task CheckAndDownloadSteamCMDAsync()
		{
			string exePath = Path.Combine(_steamCmdDir, "steamcmd.exe");
			if (!File.Exists(exePath))
			{
				await DownloadSteamCMDAsync();
				await UpdateServerAsync();
				await Task.Delay(500); // Small delay to ensure file system stability
			}
		}

		/// <summary>
		/// Downloads SteamCMD, extracts it, then runs it once to self-update.
		/// </summary>
		private async Task DownloadSteamCMDAsync()
		{
			try
			{
				AppendConsole("Downloading SteamCMD...");

				string zipUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
				string tempZipPath = Path.Combine(_steamCmdDir, "steamcmd.zip");
				string steamCmdExe = Path.Combine(_steamCmdDir, "steamcmd.exe");

				Directory.CreateDirectory(_steamCmdDir);

				using HttpClient client = new();
				using HttpResponseMessage response = await client.GetAsync(
					zipUrl,
					HttpCompletionOption.ResponseHeadersRead
				);

				response.EnsureSuccessStatusCode();

				await using (FileStream fs = new(
					tempZipPath,
					FileMode.Create,
					FileAccess.Write,
					FileShare.None))
				{
					await response.Content.CopyToAsync(fs);
				}

				AppendConsole("Download complete. Extracting SteamCMD...");
				ZipFile.ExtractToDirectory(tempZipPath, _steamCmdDir, overwriteFiles: true);
				File.Delete(tempZipPath);

				AppendConsole("SteamCMD extracted. Running first-time update...");

				await RunSteamCmdSelfUpdateAsync(steamCmdExe);

				AppendConsole("SteamCMD extracted. Running first-time update...");

				await RunSteamCmdSelfUpdateAsync(steamCmdExe);

				AppendConsole("SteamCMD extracted. Running first-time update...");

				await RunSteamCmdSelfUpdateAsync(steamCmdExe);

				AppendConsole("SteamCMD update completed successfully.");
			}
			catch (Exception ex)
			{
				AppendConsole($"Error downloading or updating SteamCMD: {ex.Message}");
			}
		}

		/// <summary>
		/// Runs SteamCMD once so it can self-update and bootstrap.
		/// </summary>
		private async Task RunSteamCmdSelfUpdateAsync(string steamCmdExe)
		{
			if (!File.Exists(steamCmdExe))
				throw new FileNotFoundException("steamcmd.exe not found after extraction.");

			using Process process = new()
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = steamCmdExe,
					Arguments = "+quit",
					WorkingDirectory = _steamCmdDir,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				},
				EnableRaisingEvents = true
			};

			process.OutputDataReceived += (s, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.Data))
					AppendConsole("[SteamCMD] " + e.Data);
			};

			process.ErrorDataReceived += (s, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.Data))
					AppendConsole("[SteamCMD][ERR] " + e.Data);
			};

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			await process.WaitForExitAsync();

			if (process.ExitCode != 0)
				throw new Exception($"SteamCMD exited with code {process.ExitCode}");
		}

		/// <summary>
		/// Starts the external server process using the configured settings. The
		/// process is launched with redirected standard output and error streams so
		/// that output can be streamed to the consoleTextBox.
		/// </summary>
		private void StartServer()
		{
			DisableButtonsOnStart();
			SaveSettingsToFile();
			try
			{
				EnsureSaveGameExists();
			}
			catch (Exception ex)
			{
				AppendConsole($"SaveGame preparation failed: {ex.Message}");
				_uiState = ServerUiState.Idle;
				UpdateButtonStates();
				return;
			}
			if (_serverManager != null)
				_serverManager.StartServer(_settings.Port);
		}


		/// <summary>
		/// Stops the server process if it is currently running.
		/// </summary>
		private void StopServer()
		{
			if (_serverManager != null)
				_serverManager.StopServer();
			if (!_isUpdating)
			{
				_uiState = ServerUiState.Idle;
				UpdateButtonStates();
			}
		}

		/// <summary>
		/// Gracefully stops the Unreal / Satisfactory server using STDIN.
		/// Falls back to Kill() if it does not exit in time.
		/// </summary>
		private async Task StopServerAsync()
		{
			if (_serverManager != null)
				await _serverManager.StopServerAsync();
			_uiState = ServerUiState.Idle;
			UpdateButtonStates();
		}

		/// <summary>
		/// Runs a SteamCMD update. Stops the server before executing SteamCMD and
		/// streams all output to the console. The update command is a placeholder
		/// and should be tailored to the specific application and app ID.
		/// </summary>
		private async Task UpdateServerAsync()
		{
			// Mark update in progress and disable all buttons
			_isUpdating = true;
			_uiState = ServerUiState.Busy;
			UpdateButtonStates();

			// Stop any running server before updating
			StopServer();

			string steamCmdExe = Path.Combine(_steamCmdDir, "steamcmd.exe");
			if (!File.Exists(steamCmdExe))
			{
				AppendConsole("SteamCMD executable not found. Please download SteamCMD first.");
				// Update done (but incomplete); re-enable controls
				_isUpdating = false;
				_uiState = ServerUiState.Idle;
				UpdateButtonStates();
				return;
			}

			// Example SteamCMD command: login anonymously and quit. Replace with
			// actual commands to update your server (e.g., app_update <id> validate).
			string arguments = $"+force_install_dir {_serverPath} +login anonymous +app_update 3809400 validate +quit";

			var psi = new ProcessStartInfo
			{
				FileName = steamCmdExe,
				Arguments = arguments,
				WorkingDirectory = _steamCmdDir,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			AppendConsole("Starting SteamCMD update...");

			using var proc = new Process { StartInfo = psi };
			proc.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendConsole(e.Data!); };
			proc.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendConsole(e.Data!); };
			proc.Start();
			proc.BeginOutputReadLine();
			proc.BeginErrorReadLine();
			await Task.Delay(500); // Small delay to ensure all output is processed
			await proc.WaitForExitAsync();
			await Task.Delay(500); // Small delay to ensure all output is processed
			AppendConsole("SteamCMD update completed.");

			// Update done; re-enable buttons
			_isUpdating = false;
			_uiState = ServerUiState.Idle;
			UpdateButtonStates();
		}

		/// <summary>
		/// Starts the auto-update timer. The timer runs asynchronously at the
		/// configured interval, checking for updates and restarting the server if needed.
		/// </summary>
		private void StartAutoUpdateTimer()
		{
			var timespan = TimeSpan.FromMinutes(30);
			// Dispose any existing timer to avoid multiple instances
			_autoUpdateTimer?.Dispose();
			_autoUpdateTimer = new Timer(async _ => await AutoUpdateAsync(), null, timespan, timespan);
			AppendConsole($"Auto-update timer started. Interval: {_settings.UpdateInterval} minutes.");
		}

		/// <summary>
		/// Called by the auto-update timer. Performs an update via SteamCMD and
		/// restarts the server if it was running prior to the update.
		/// </summary>
		private async Task AutoUpdateAsync()
		{
			//SetBusyState(true);

			// Use the renamed autoUpdateCheckBox instead of the default-named checkBox1
			if (autoUpdateCheckBox.Checked == false)
			{
				AppendConsole("Auto-update Disabled.");
				return;
			}

			// Prevent overlap
			if (_isUpdating)
				return;

			_isUpdating = true;

			try
			{
				bool serverWasRunning = _serverManager != null && _serverManager.IsRunning;

				await InvokeOnUiAsync(() =>
				{
					AppendConsole("Auto-update check triggered.");
					_uiState = ServerUiState.Busy;
					UpdateButtonStates();
				});

				// 👉 here is where you check IF an update exists
				bool updateAvailable = await IsUpdateAvailableAsync();

				if (!updateAvailable)
				{
					await InvokeOnUiAsync(() =>
						AppendConsole("No update available.")
					);
					return;
				}

				await InvokeOnUiAsync(() =>
					AppendConsole("Update available – applying...")
				);

				if (serverWasRunning)
					await StopServerAsync();

				await UpdateServerAsync();

				if (serverWasRunning)
					await InvokeOnUiAsync(StartServer);
			}
			finally
			{
				_isUpdating = false;
				//SetBusyState(false);

				_uiState = ServerUiState.Idle;
				UpdateButtonStates();
			}
			//SetBusyState(false);
		}

		private void EnsureSaveGameExists()
		{
			if (string.IsNullOrWhiteSpace(_settings.SessionName))
				throw new InvalidOperationException("SessionName is not set.");

			string saveGameDir = Path.Combine(
				_serverPath,
				"StarRupture",
				"Saved",
				"SaveGames",
				_settings.SessionName
			);

			// Create directory if missing
			Directory.CreateDirectory(saveGameDir);

			string targetFile1 = Path.Combine(saveGameDir, "AutoSave0.met");
			string targetFile2 = Path.Combine(saveGameDir, "AutoSave0.sav");

			// Only copy if it does not already exist
			if (!File.Exists(targetFile1))
			{
				var asm = Assembly.GetExecutingAssembly();

				// ⚠️ Update namespace if your project name differs
				const string resourceName = "RuptureServerManager.AutoSaveEmpty.AutoSave0.met";

				using Stream? resourceStream = asm.GetManifestResourceStream(resourceName);
				if (resourceStream == null)
					throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

				using FileStream fs = new FileStream(targetFile1, FileMode.Create, FileAccess.Write);
				resourceStream.CopyTo(fs);
			}

			// Only copy if it does not already exist
			if (!File.Exists(targetFile2))
			{
				var asm = Assembly.GetExecutingAssembly();

				// ⚠️ Update namespace if your project name differs
				const string resourceName = "RuptureServerManager.AutoSaveEmpty.AutoSave0.sav";

				using Stream? resourceStream = asm.GetManifestResourceStream(resourceName);
				if (resourceStream == null)
					throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

				using FileStream fs = new FileStream(targetFile2, FileMode.Create, FileAccess.Write);
				resourceStream.CopyTo(fs);
			}
		}


		private async Task<bool> IsUpdateAvailableAsync()
		{
			try
			{
				int localBuild = GetLocalBuildId();
				int remoteBuild = await GetRemoteBuildIdAsync();

				if (localBuild == 0 || remoteBuild == 0)
				{
					AppendConsole("Unable to determine build IDs – skipping update.");
					return false;
				}

				AppendConsole($"Local build: {localBuild}, Remote build: {remoteBuild}");

				return remoteBuild > localBuild;
			}
			catch (Exception ex)
			{
				AppendConsole($"Update check failed: {ex.Message}");
				return false;
			}
		}

		private int GetLocalBuildId()
		{
			try
			{
				string manifestPath = Path.Combine(
					_serverPath,
					"steamapps",
					$"appmanifest_{_steamAppId}.acf");

				if (!File.Exists(manifestPath))
					return 0;

				foreach (string line in File.ReadLines(manifestPath))
				{
					if (line.Trim().StartsWith("\"buildid\""))
					{
						string value = line.Split('"', StringSplitOptions.RemoveEmptyEntries)[3];
						return int.TryParse(value, out int build) ? build : 0;
					}
				}
			}
			catch { }

			return 0;
		}

		private async Task<int> GetRemoteBuildIdAsync()
		{
			string steamCmdExe = Path.Combine(_steamCmdDir, "steamcmd.exe");
			if (!File.Exists(steamCmdExe))
				return 0;

			var psi = new ProcessStartInfo
			{
				FileName = steamCmdExe,
				Arguments = $"+login anonymous +app_info_update 1 +app_info_print {_steamAppId} +quit",
				WorkingDirectory = _steamCmdDir,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using var proc = Process.Start(psi);
			if (proc == null)
				return 0;

			string output = await proc.StandardOutput.ReadToEndAsync();
			await proc.WaitForExitAsync();

			foreach (string line in output.Split('\n'))
			{
				if (line.Contains("\"buildid\""))
				{
					string value = line.Split('"', StringSplitOptions.RemoveEmptyEntries)[3];
					return int.TryParse(value, out int build) ? build : 0;
				}
			}

			return 0;
		}

		/// <summary>
		/// Executes an action on the UI thread asynchronously.
		/// Safe to call from timers, background threads, and async tasks.
		/// </summary>
		private Task InvokeOnUiAsync(Action action)
		{
			if (IsDisposed || !IsHandleCreated)
				return Task.CompletedTask;

			var tcs = new TaskCompletionSource();

			BeginInvoke(new Action(() =>
			{
				try
				{
					action();
					tcs.SetResult();
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			}));

			return tcs.Task;
		}

		/// <summary>
		/// Appends a message to the consoleTextBox with a timestamp. Ensures that
		/// cross-thread calls are marshaled onto the UI thread when invoked from
		/// background operations.
		/// </summary>
		/// <param name="message">Message to append.</param>
		private void AppendConsole(string message)
		{
			if (consoleTextBox.InvokeRequired)
			{
				consoleTextBox.Invoke(new Action<string>(AppendConsole), message);
				return;
			}
			consoleTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
			consoleTextBox.SelectionStart = consoleTextBox.TextLength;
			consoleTextBox.SelectionLength = 0;
			consoleTextBox.ScrollToCaret();
			Logger.Log(message);
		}

		// Method removed; replaced by Logger.Log(message)

		#region Event Handlers

		private void SaveSettingsButton_Click(object? sender, EventArgs e)
		{
			SaveSettingsToFile();
		}

		private async void DownloadSteamCmdButton_Click(object? sender, EventArgs e)
		{
			await DownloadSteamCMDAsync();
		}

		private void StartButton_Click(object? sender, EventArgs e)
		{
			StartServer();
		}

		private async void StopButton_Click(object? sender, EventArgs e)
		{
			await StopServerAsync();
		}

		private async void UpdateButton_Click(object? sender, EventArgs e)
		{
			await UpdateServerAsync();
		}

		#endregion

		private void UpdateInterval_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Allow control keys (Backspace, Delete, Ctrl+C/V, etc.)
			if (char.IsControl(e.KeyChar))
				return;

			// Allow digits only
			if (!char.IsDigit(e.KeyChar))
				e.Handled = true;
		}

		private async Task SendServerCommandAsync(string command)
		{
			if (_serverManager != null)
				await _serverManager.SendServerCommandAsync(command);
		}

		/// <summary>
		/// Handles click events for both the admin and player password set buttons.
		/// Invokes the asynchronous update routine to post the current password
		/// values and write the encrypted results to disk.
		/// </summary>
		private async void SetPasswordButton_Click(object? sender, EventArgs e)
		{
			await OnSetPasswordsAsync();
		}

		/// <summary>
		/// Reads the current values from the admin and player password text boxes and
		/// sends them to the StarRupture password API.  Any exceptions are logged
		/// to the console.  A message will be appended to the console on success.
		/// </summary>
		private async Task OnSetPasswordsAsync()
		{
			try
			{
				// Read masked password text from the UI controls. Null‑coalesce to empty strings.
				string adminPassword = adminPasswordTextBox?.Text ?? string.Empty;
				string playerPassword = playerPasswordTextBox?.Text ?? string.Empty;
				await UpdatePasswordsAsync(adminPassword, playerPassword);
				AppendConsole("Passwords updated successfully.");
			}
			catch (Exception ex)
			{
				AppendConsole($"Failed to update passwords: {ex.Message}");
			}
		}

		/// <summary>
		/// Calls the password encryption API using HTTP POST form data and persists
		/// the returned encrypted strings to files.  The administrator password
		/// is stored in <c>Password.json</c> and the player password in
		/// <c>ServerPassword.json</c> under the server path.
		/// </summary>
		/// <param name="adminPassword">Clear‑text administrator password entered by the user.</param>
		/// <param name="playerPassword">Clear‑text player password entered by the user.</param>
		private async Task UpdatePasswordsAsync(string adminPassword, string playerPassword)
		{
			using var httpClient = new HttpClient();
			using var content = new MultipartFormDataContent();
			// Always send both values, even if blank, to match the API contract.
			content.Add(new StringContent(adminPassword ?? string.Empty), "adminpassword");
			content.Add(new StringContent(playerPassword ?? string.Empty), "playerpassword");

			// Post the form data to the API endpoint.  Throws on non‑success status codes.
			using HttpResponseMessage response = await httpClient.PostAsync("https://starrupture.agngaming.com/passwords/", content);
			response.EnsureSuccessStatusCode();
			string json = await response.Content.ReadAsStringAsync();

			// Parse the returned JSON and write each value to its respective file.
			using JsonDocument document = JsonDocument.Parse(json);
			if (document.RootElement.TryGetProperty("adminpassword", out JsonElement adminElement))
			{
				string encrypted = adminElement.GetString() ?? string.Empty;
				WritePasswordFile("Password.json", $"{{\"password\":\"{encrypted}\"}}");
			}
			if (document.RootElement.TryGetProperty("playerpassword", out JsonElement playerElement))
			{
				string encrypted = playerElement.GetString() ?? string.Empty;
				WritePasswordFile("PlayerPassword.json", $"{{\"password\":\"{encrypted}\"}}");
			}
		}

		/// <summary>
		/// Writes the supplied encrypted password value to a file in the server path.
		/// The directories are created if they do not already exist.
		/// </summary>
		/// <param name="fileName">The filename (e.g. Password.json or ServerPassword.json)</param>
		/// <param name="encryptedValue">The encrypted string returned by the API.</param>
		private void WritePasswordFile(string fileName, string encryptedValue)
		{
			try
			{
				if (string.IsNullOrEmpty(encryptedValue))
					return;
				Directory.CreateDirectory(_serverPath);
				var fullPath = Path.Combine(_serverPath, fileName);
				File.WriteAllText(fullPath, encryptedValue);
			}
			catch (Exception ex)
			{
				AppendConsole($"Failed to write password file {fileName}: {ex.Message}");
			}
		}
	}
}