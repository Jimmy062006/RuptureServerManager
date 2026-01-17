using System;
using System.IO;
using System.Text.Json;

namespace RuptureServerManager
{
    public class ConfigManager<T> where T : class, new()
    {
        private readonly string _filePath;
        private readonly Action<string> _logger;

        public ConfigManager(string filePath, Action<string> logger)
        {
            _filePath = filePath;
            _logger = logger;
        }

        public T Load()
        {
            if (!File.Exists(_filePath))
                return new T();
            try
            {
                string json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<T>(json);
                return loaded ?? new T();
            }
            catch (Exception ex)
            {
                _logger($"Error loading config: {ex.Message}");
                return new T();
            }
        }

        public void Save(T config)
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_filePath, json);
                _logger("Config saved.");
            }
            catch (Exception ex)
            {
                _logger($"Error saving config: {ex.Message}");
            }
        }
    }
}
