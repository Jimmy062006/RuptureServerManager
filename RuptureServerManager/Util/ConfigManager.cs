using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace RuptureServerManager.Util
{
    public class ConfigManager
    {
        public static ConfigManager Instance => _instance ??= new();
        private static ConfigManager? _instance;

        private readonly string _filePath;
        private Action<string>? _logger;
        private RuptureServerManagerSettings _config = new RuptureServerManagerSettings();

        public ConfigManager()
        {
            var _appFolder = Path.Combine(Application.StartupPath, "config");
            if ( !Directory.Exists(_appFolder) )
            {
                try
                {
                    Directory.CreateDirectory(_appFolder);
                } catch (Exception ex)
                {
                    throw new Exception($"Unable to create new directory {_appFolder}, error: {ex.Message}.  Please ensure you have the needed permissions.");
                }
            }
            _filePath = Path.Combine(_appFolder, "RuptureServerManagerSettings.txt");

            Load();
        }

        public void AssignLogger(Action<string> logger)
        {
            _logger = logger;
        }

        public RuptureServerManagerSettings GetConfig()
        {
            return _config;
        }

        private void Load()
        {
            RuptureServerManagerSettings? configBlob = null;
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                configBlob = JsonSerializer.Deserialize<RuptureServerManagerSettings>(json);
            }

            _config = configBlob ?? new RuptureServerManagerSettings();
        }

        public void Save()
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true };
                string json = JsonSerializer.Serialize(_config, options);
                File.WriteAllText(_filePath, json);
                _logger?.Invoke("Config saved.");
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Error saving config: {ex.Message}");
            }
        }
    }
}
