using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace RuptureServerManager.Services
{
	internal class UpdateChecker
	{
		private const string GITHUB_API_URL =
			"https://api.github.com/repos/Jimmy062006/RuptureServerManager/releases/latest";

		private static readonly HttpClient _httpClient = new()
		{
			DefaultRequestHeaders =
			{
				{ "User-Agent", "RuptureServerManager" }
			}
		};

		public async Task<UpdateInfo?> CheckForUpdatesAsync()
		{
			try
			{
				var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
				using var release = JsonDocument.Parse(response);
				var root = release.RootElement;

				string latestVersion = root.GetProperty("tag_name")
					.GetString()?
					.TrimStart('v') ?? "0.0.0";

				string currentVersion = GetCurrentVersion();

				return new UpdateInfo
				{
					CurrentVersion = currentVersion,
					LatestVersion = latestVersion,
					UpdateAvailable = IsNewerVersion(latestVersion, currentVersion),
					DownloadUrl = root.GetProperty("html_url").GetString() ?? "",
					ReleaseNotes = root.GetProperty("body").GetString() ?? ""
				};
			}
			catch
			{
				return null;
			}
		}

		public async Task<bool> DownloadAndApplyUpdateAsync(UpdateInfo info)
		{
			if (!info.UpdateAvailable)
				return false;

			string tempRoot = Path.Combine(Path.GetTempPath(), "RuptureServerManagerUpdate");
			string zipPath = Path.Combine(tempRoot, "update.zip");
			string extractPath = Path.Combine(tempRoot, "extract");

			Directory.CreateDirectory(tempRoot);

			string? zipUrl = await GetZipAssetUrlAsync();
			if (zipUrl == null)
				return false;

			using var request = new HttpRequestMessage(HttpMethod.Get, zipUrl);
			request.Headers.UserAgent.Add(
				new ProductInfoHeaderValue("RuptureServerManager", "1.0"));

			using var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			await using (var fs = File.Create(zipPath))
			{
				await response.Content.CopyToAsync(fs);
			}

			if (Directory.Exists(extractPath))
				Directory.Delete(extractPath, true);

			ZipFile.ExtractToDirectory(zipPath, extractPath);

			string updateSource = ResolveZipRoot(extractPath);
			LaunchUpdater(updateSource);

			return true;
		}

		private static string ResolveZipRoot(string extractPath)
		{
			var dirs = Directory.GetDirectories(extractPath);
			return dirs.Length == 1 ? dirs[0] : extractPath;
		}

		private void LaunchUpdater(string updateSourceDir)
		{
			string currentExe = Environment.ProcessPath!;
			string appDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
			int pid = Environment.ProcessId;

			Process.Start(new ProcessStartInfo
			{
				FileName = currentExe,
				Arguments = $"--update \"{appDir}\" \"{updateSourceDir}\" {pid}",
				UseShellExecute = true
			});

			Environment.Exit(0);
		}

		private async Task<string?> GetZipAssetUrlAsync()
		{
			var json = await _httpClient.GetStringAsync(GITHUB_API_URL);
			using var doc = JsonDocument.Parse(json);

			foreach (var asset in doc.RootElement.GetProperty("assets").EnumerateArray())
			{
				string name = asset.GetProperty("name").GetString() ?? "";
				if (name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					return asset.GetProperty("browser_download_url").GetString();
				}
			}

			return null;
		}

		private string GetCurrentVersion()
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			return version == null
				? "0.0.0"
				: $"{version.Major}.{version.Minor}.{version.Build}";
		}

		private bool IsNewerVersion(string latest, string current)
		{
			try
			{
				var l = latest.Split('.').Select(int.Parse).ToArray();
				var c = current.Split('.').Select(int.Parse).ToArray();

				for (int i = 0; i < Math.Min(l.Length, c.Length); i++)
				{
					if (l[i] > c[i]) return true;
					if (l[i] < c[i]) return false;
				}

				return l.Length > c.Length;
			}
			catch
			{
				return false;
			}
		}
	}

	public class UpdateInfo
	{
		public string CurrentVersion { get; set; } = "";
		public string LatestVersion { get; set; } = "";
		public bool UpdateAvailable { get; set; }
		public string DownloadUrl { get; set; } = "";
		public string ReleaseNotes { get; set; } = "";
	}
}
