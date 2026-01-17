using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuptureServerManager.Server
{
    public class ServerManager
    {
        public static ServerManager Instance => _instance ??= new();
        private static ServerManager? _instance;

        private Process? _serverProcess;
        private readonly string _serverPath;
        private Action<string>? _logger;

        public ServerManager()
        {
            var _folder = Path.Combine(Application.StartupPath, "server");
            if (!Directory.Exists(_folder))
            {
                try
                {
                    Directory.CreateDirectory(_folder);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to create new directory {_folder}, error: {ex.Message}.  Please ensure you have the needed permissions.");
                }
            }
            _serverPath = _folder;
        }

        public string GetServerPath() => _serverPath;
        public bool IsRunning => _serverProcess != null && !_serverProcess.HasExited;

        public void AssignLogger(Action<string> logger)
        {
            _logger = logger;
        }

        public bool IsServerInstalled()
        {
            string serverExe = Path.Combine(_serverPath, "StarRuptureServerEOS.exe");
            return File.Exists(serverExe);
        }

        public bool StartServer()
        {
            if (IsRunning)
            {
                _logger?.Invoke("Server is already running.");
                return true;
            }

            var port = Util.ConfigManager.Instance.GetConfig().Port;

            string serverExe = Path.Combine(_serverPath, "StarRuptureServerEOS.exe");
            string args = $"-port={port} -RCWebControlDisable -RCWebInterfaceDisable";
            var psi = new ProcessStartInfo
            {
                FileName = serverExe,
                Arguments = args,
                WorkingDirectory = _serverPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                _serverProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
                _serverProcess.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger?.Invoke(e.Data!); };
                _serverProcess.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger?.Invoke(e.Data!); };
                _serverProcess.Exited += (s, e) => { _logger?.Invoke("Server process exited."); };
                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
                _logger?.Invoke("Server started.");
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Failed to start server: {ex.Message}");
                _serverProcess = null;
            }

            return true;
        }

        public void StopServer()
        {
            if (IsRunning)
            {
                _logger?.Invoke("Stopping server...");
                try
                {
                    _serverProcess?.Kill();
                    _serverProcess?.WaitForExit();
                    _logger?.Invoke("Server stopped.");
                }
                catch (Exception ex)
                {
                    _logger?.Invoke($"Error stopping server: {ex.Message}");
                }
            }
            else
            {
                _logger?.Invoke("Server is not running.");
            }
        }

        public async Task StopServerAsync()
        {
            if (!IsRunning)
            {
                _logger?.Invoke("Server is not running.");
                return;
            }

            _logger?.Invoke("Requesting server shutdown...");
            try
            {
                if (_serverProcess!.StartInfo.RedirectStandardInput && !_serverProcess.StandardInput.BaseStream.CanWrite)
                {
                    _logger?.Invoke("STDIN already closed; waiting for server to exit...");
                }
                else
                {
                    await _serverProcess.StandardInput.WriteLineAsync("quit");
                    await _serverProcess.StandardInput.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Shutdown command send failed (expected): {ex.Message}");
            }

            _logger?.Invoke("Waiting for server to stop (saving may take time)...");
            bool exited = await Task.Run(() => _serverProcess!.WaitForExit(10000));
            if (exited)
            {
                _logger?.Invoke("Server exited cleanly.");
            }
            else
            {
                _logger?.Invoke("Server did not exit in time. Forcing shutdown...");
                _serverProcess!.Kill(true);
                await _serverProcess.WaitForExitAsync();
                _logger?.Invoke("Server forcefully stopped.");
            }
        }

        public async Task SendServerCommandAsync(string command)
        {
            if (!IsRunning)
            {
                _logger?.Invoke("Server is not running.");
                return;
            }
            try
            {
                _logger?.Invoke($"> {command}");
                await _serverProcess!.StandardInput.WriteLineAsync(command);
                await _serverProcess.StandardInput.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Failed to send command: {ex.Message}");
            }
        }
    }
}
