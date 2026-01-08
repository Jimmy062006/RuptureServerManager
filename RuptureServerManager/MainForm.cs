using RuptureServerManagerSettingsNS;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuptureServerManager
{
    /// <summary>
    /// Main form for the dedicated server controller. Handles UI events,
    /// persistence of settings, and launching/updating external processes.
    /// </summary>
    public partial class MainForm : Form
    {
        private RuptureServerManagerSettings _settings = new();
        private Process? _serverProcess;
        private string _appFolder = string.Empty;
        private string _serverPath = string.Empty;
        private string _settingsFilePath = string.Empty;
        private string _steamCmdDir = string.Empty;
		private readonly string logFilePath = Path.Combine(AppContext.BaseDirectory, "logs");
		private readonly string logFileName = "server.txt";

		private const int CTRL_C_EVENT = 0;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GenerateConsoleCtrlEvent(int dwCtrlEvent, int dwProcessGroupId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(IntPtr handler, bool add);


		/// <summary>
		/// Constructor. Initializes UI components defined in the designer file.
		/// </summary>
		public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads persisted settings and ensures SteamCMD is available when the form loads.
        /// </summary>
        private async void MainForm_Load(object? sender, EventArgs e)
        {
            InitializePaths();
            LoadSettingsFromFile();
            ApplySettingsToUi();
            await CheckAndDownloadSteamCMDAsync();
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
        /// defaults are used. Port is stored separately and loaded from the UI to
        /// accommodate the [JsonIgnore] attribute.
        /// </summary>
        private void LoadSettingsFromFile()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<RuptureServerManagerSettings>(json);
                    if (loaded != null)
                    {
                        _settings = loaded;
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"Error loading settings: {ex.Message}");
                    // Use defaults if loading fails
                    _settings = new RuptureServerManagerSettings();
                }
            }
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
            startNewGameCheckBox.Checked = _settings.StartNewGame;
            loadSavedGameCheckBox.Checked = _settings.LoadSavedGame;
            saveGameNameTextBox.Text = _settings.SaveGameName;
        }

        /// <summary>
        /// Writes the current settings object to disk as JSON. The port value is
        /// updated from the UI before saving.
        /// </summary>
        private void SaveSettingsToFile()
        {
            UpdateSettingsFromUi();
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);
                File.WriteAllText(Path.Combine(_serverPath, "RuptureServerManagerSettings.txt"), json);
                AppendConsole("Settings saved.");
            }
            catch (Exception ex)
            {
                AppendConsole($"Error saving settings: {ex.Message}");
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
            _settings.StartNewGame = startNewGameCheckBox.Checked;
            _settings.LoadSavedGame = loadSavedGameCheckBox.Checked;
            _settings.SaveGameName = saveGameNameTextBox.Text.Trim();
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

				using HttpClient client = new HttpClient();
				using HttpResponseMessage response = await client.GetAsync(
					zipUrl,
					HttpCompletionOption.ResponseHeadersRead
				);

				response.EnsureSuccessStatusCode();

				await using (FileStream fs = new FileStream(
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

			using Process process = new Process
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
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                AppendConsole("Server is already running.");
                return;
            }

            SaveSettingsToFile();
            // Determine the path to the server executable. By default we assume
            // there is a file named 'server.exe' alongside the application. Users
            // may replace this with the actual executable for their server.
            string serverExe = Path.Combine(_serverPath, "StarRuptureServerEOS.exe");
            if (!File.Exists(serverExe))
            {
                AppendConsole("Server executable not found. Please place your server binary as 'server.exe' next to the application.");
                return;
            }

            // Compose command line arguments. These arguments are examples and
            // should be adapted to the actual server's command line options.
            string args = $"-port={_settings.Port}";

            var psi = new ProcessStartInfo
            {
                FileName = serverExe,
                Arguments = args,
                WorkingDirectory = Application.StartupPath,
                UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                _serverProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
                _serverProcess.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendConsole(e.Data!); };
                _serverProcess.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendConsole(e.Data!); };
                _serverProcess.Exited += (s, e) => { AppendConsole("Server process exited."); };

                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
                AppendConsole("Server started.");
            }
            catch (Exception ex)
            {
                AppendConsole($"Failed to start server: {ex.Message}");
                _serverProcess = null;
            }
        }

        /// <summary>
        /// Stops the server process if it is currently running.
        /// </summary>
        private void StopServer()
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                try
                {
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit();
                    AppendConsole("Server stopped.");
                }
                catch (Exception ex)
                {
                    AppendConsole($"Error stopping server: {ex.Message}");
                }
            }
            else
            {
                AppendConsole("Server is not running.");
            }
        }

		/// <summary>
		/// Gracefully stops the Unreal / Satisfactory server using STDIN.
		/// Falls back to Kill() if it does not exit in time.
		/// </summary>
		private async Task StopServerAsync()
		{
			if (_serverProcess == null || _serverProcess.HasExited)
			{
				AppendConsole("Server is not running.");
				return;
			}

			AppendConsole("Stopping server (graceful shutdown)...");

			try
			{
				// Send Unreal's built-in quit command
				if (_serverProcess.StartInfo.RedirectStandardInput)
				{
					await _serverProcess.StandardInput.WriteLineAsync("quit");
					await _serverProcess.StandardInput.FlushAsync();
				}
				else
				{
					AppendConsole("Warning: STDIN is not redirected. Cannot send quit command.");
				}

				// Unreal servers can take time to save
				bool exited = await Task.Run(() => _serverProcess.WaitForExit(30000));

				if (exited)
				{
					AppendConsole("Server stopped cleanly.");
				}
				else
				{
					AppendConsole("Server did not exit in time. Forcing shutdown...");
					_serverProcess.Kill(true);
					await _serverProcess.WaitForExitAsync();
					AppendConsole("Server forcefully stopped.");
				}
			}
			catch (Exception ex)
			{
				AppendConsole($"Error stopping server: {ex.Message}");
			}
		}

		/// <summary>
		/// Runs a SteamCMD update. Stops the server before executing SteamCMD and
		/// streams all output to the console. The update command is a placeholder
		/// and should be tailored to the specific application and app ID.
		/// </summary>
		private async Task UpdateServerAsync()
        {
            // Stop any running server before updating
            StopServer();

            string steamCmdExe = Path.Combine(_steamCmdDir, "steamcmd.exe");
            if (!File.Exists(steamCmdExe))
            {
                AppendConsole("SteamCMD executable not found. Please download SteamCMD first.");
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
            await proc.WaitForExitAsync();
            AppendConsole("SteamCMD update completed.");
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

			// Auto-scroll to bottom
			consoleTextBox.SelectionStart = consoleTextBox.TextLength;
			consoleTextBox.SelectionLength = 0;
			consoleTextBox.ScrollToCaret();

			// Also write to file
			LogToFile(message);
		}

		private void LogToFile(string message)
		{
            try
            {
                if (!Directory.Exists(logFilePath))
                {
                    Directory.CreateDirectory(logFilePath);
                }

                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(Path.Combine(logFilePath, logFileName), line + Environment.NewLine);
            }
            catch
            {
                // Intentionally swallow logging errors
                // (we never want logging to crash the app)
            }
		}

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
            //StopServer();
            await StopServerAsync();
        }

        private async void UpdateButton_Click(object? sender, EventArgs e)
        {
            await UpdateServerAsync();
        }

        #endregion
    }
}