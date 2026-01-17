using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RuptureServerManager
{
    public class ServerManager
    {
        private Process? _serverProcess;
        private readonly string _serverPath;
        private readonly Action<string> _logger;

        public ServerManager(string serverPath, Action<string> logger)
        {
            _serverPath = serverPath;
            _logger = logger;
        }

        public bool IsRunning => _serverProcess != null && !_serverProcess.HasExited;

        public void StartServer(int port)
        {
            if (IsRunning)
            {
                _logger("Server is already running.");
                return;
            }

            string serverExe = Path.Combine(_serverPath, "StarRuptureServerEOS.exe");
            if (!File.Exists(serverExe))
            {
                _logger("Server executable not found.");
                return;
            }

            string args = $"-port={port} -log -RCWebControlDisable -RCWebInterfaceDisable";
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
                _serverProcess.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger(e.Data!); };
                _serverProcess.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger(e.Data!); };
                _serverProcess.Exited += (s, e) => { _logger("Server process exited."); };
                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
                _logger("Server started.");
            }
            catch (Exception ex)
            {
                _logger($"Failed to start server: {ex.Message}");
                _serverProcess = null;
            }
        }

        public void StopServer()
        {
            if (IsRunning)
            {
                try
                {
                    _serverProcess!.Kill();
                    _serverProcess.WaitForExit();
                    _logger("Server stopped.");
                }
                catch (Exception ex)
                {
                    _logger($"Error stopping server: {ex.Message}");
                }
            }
            else
            {
                _logger("Server is not running.");
            }
        }

        public async Task StopServerAsync()
        {
            if (!IsRunning)
            {
                _logger("Server is not running.");
                return;
            }

            _logger("Requesting server shutdown...");
            try
            {
                if (_serverProcess!.StartInfo.RedirectStandardInput && !_serverProcess.StandardInput.BaseStream.CanWrite)
                {
                    _logger("STDIN already closed; waiting for server to exit...");
                }
                else
                {
                    await _serverProcess.StandardInput.WriteLineAsync("quit");
                    await _serverProcess.StandardInput.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                _logger($"Shutdown command send failed (expected): {ex.Message}");
            }

            _logger("Waiting for server to stop (saving may take time)...");
            bool exited = await Task.Run(() => _serverProcess!.WaitForExit(10000));
            if (exited)
            {
                _logger("Server exited cleanly.");
            }
            else
            {
                _logger("Server did not exit in time. Forcing shutdown...");
                _serverProcess!.Kill(true);
                await _serverProcess.WaitForExitAsync();
                _logger("Server forcefully stopped.");
            }
        }

        public async Task SendServerCommandAsync(string command)
        {
            if (!IsRunning)
            {
                _logger("Server is not running.");
                return;
            }
            try
            {
                _logger($"> {command}");
                await _serverProcess!.StandardInput.WriteLineAsync(command);
                await _serverProcess.StandardInput.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger($"Failed to send command: {ex.Message}");
            }
        }
    }
}
