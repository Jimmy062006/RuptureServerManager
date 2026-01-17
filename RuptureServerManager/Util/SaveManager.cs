using RuptureServerManager.Server;
using System;
using System.IO;
using System.Reflection;

namespace RuptureServerManager.Util
{
    internal class SaveManager
    {
        public static SaveManager Instance => _instance ??= new();
        private static SaveManager? _instance;

        public void EnsureSaveGameExists()
        {
            var _settings = ConfigManager.Instance.GetConfig();

            if (string.IsNullOrWhiteSpace(_settings.SessionName))
                throw new InvalidOperationException("SessionName is not set.");

            string saveGameDir = Path.Combine(
                ServerManager.Instance.GetServerPath(),
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
                File.WriteAllBytes(targetFile1, Properties.Resources.AutoSave0_met);
            }

            // Only copy if it does not already exist
            if (!File.Exists(targetFile2))
            {
                File.WriteAllBytes(targetFile2, Properties.Resources.AutoSave0_Sav);
            }
        }

        /// <summary>
        /// Can be used to import save files from client to server
        /// Automatically detects a met and sav file in the same folder as the provided sourceFilePath  
        /// </summary>
        /// <param name="sourceFilePath">The source file path, user should be able to pick a .sav file</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public void ImportSaveGame(string sourceFilePath)
        {
            var _settings = ConfigManager.Instance.GetConfig();
            if (string.IsNullOrWhiteSpace(_settings.SessionName))
                throw new InvalidOperationException("SessionName is not set.");
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("Source save game file not found.", sourceFilePath);

            string saveGameDir = Path.Combine(
                ServerManager.Instance.GetServerPath(),
                "StarRupture",
                "Saved",
                "SaveGames",
                _settings.SessionName
            );

            Directory.CreateDirectory(saveGameDir);

            var _folderPath = Path.GetDirectoryName(sourceFilePath);
            if ( File.Exists(Path.Combine(_folderPath!, "AutoSave0.met")) )
            {
                string sourceMetPath = Path.Combine(_folderPath!, "AutoSave0.met");
                string targetMetPath = Path.Combine(saveGameDir, "AutoSave0.met");
                File.Copy(sourceMetPath, targetMetPath, overwrite: true);
            }

            if ( File.Exists(Path.Combine(_folderPath!, "AutoSave0.sav")) )
            {
                string sourceSavPath = Path.Combine(_folderPath!, "AutoSave0.sav");
                string targetSavPath = Path.Combine(saveGameDir, "AutoSave0.sav");
                File.Copy(sourceSavPath, targetSavPath, overwrite: true);
            }
        }

    }
}
