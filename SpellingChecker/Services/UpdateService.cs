using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for checking and applying application updates from GitHub releases
    /// </summary>
    public class UpdateService
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/trollgameskr/SpellingChecker/releases/latest";
        private static readonly HttpClient _httpClient = new HttpClient();

        static UpdateService()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SpellingChecker-Auto-Updater");
        }

        public class UpdateInfo
        {
            public string? LatestVersion { get; set; }
            public string? CurrentVersion { get; set; }
            public bool UpdateAvailable { get; set; }
            public string? DownloadUrl { get; set; }
            public string? ReleaseNotes { get; set; }
        }

        private class GitHubRelease
        {
            [JsonPropertyName("tag_name")]
            public string? TagName { get; set; }

            [JsonPropertyName("body")]
            public string? Body { get; set; }

            [JsonPropertyName("assets")]
            public GitHubAsset[]? Assets { get; set; }
        }

        private class GitHubAsset
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("browser_download_url")]
            public string? BrowserDownloadUrl { get; set; }
        }

        /// <summary>
        /// Get current application version
        /// </summary>
        public static string GetCurrentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v0.0.0";
        }

        /// <summary>
        /// Check if an update is available
        /// </summary>
        public static async Task<UpdateInfo> CheckForUpdatesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
                var release = JsonSerializer.Deserialize<GitHubRelease>(response);

                if (release == null || string.IsNullOrEmpty(release.TagName))
                {
                    return new UpdateInfo
                    {
                        CurrentVersion = GetCurrentVersion(),
                        UpdateAvailable = false
                    };
                }

                var currentVersion = GetCurrentVersion();
                var latestVersion = release.TagName;
                var updateAvailable = CompareVersions(currentVersion, latestVersion) < 0;

                // Find the Windows x64 ZIP asset
                string? downloadUrl = null;
                if (release.Assets != null)
                {
                    foreach (var asset in release.Assets)
                    {
                        if (asset.Name?.EndsWith("win-x64.zip") == true)
                        {
                            downloadUrl = asset.BrowserDownloadUrl;
                            break;
                        }
                    }
                }

                return new UpdateInfo
                {
                    CurrentVersion = currentVersion,
                    LatestVersion = latestVersion,
                    UpdateAvailable = updateAvailable,
                    DownloadUrl = downloadUrl,
                    ReleaseNotes = release.Body
                };
            }
            catch (Exception)
            {
                // Return no update available on error
                return new UpdateInfo
                {
                    CurrentVersion = GetCurrentVersion(),
                    UpdateAvailable = false
                };
            }
        }

        /// <summary>
        /// Compare two version strings (e.g., "v1.1.15" vs "v1.1.14")
        /// Returns: -1 if version1 < version2, 0 if equal, 1 if version1 > version2
        /// </summary>
        private static int CompareVersions(string version1, string version2)
        {
            // Remove 'v' prefix if present
            version1 = version1.TrimStart('v');
            version2 = version2.TrimStart('v');

            var parts1 = version1.Split('.');
            var parts2 = version2.Split('.');

            int maxLength = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int v1 = i < parts1.Length && int.TryParse(parts1[i], out int val1) ? val1 : 0;
                int v2 = i < parts2.Length && int.TryParse(parts2[i], out int val2) ? val2 : 0;

                if (v1 < v2) return -1;
                if (v1 > v2) return 1;
            }

            return 0;
        }

        /// <summary>
        /// Download and install the update
        /// </summary>
        public static async Task<bool> DownloadAndInstallUpdateAsync(string downloadUrl, IProgress<int>? progress = null)
        {
            try
            {
                // Create temp directory for download
                var tempDir = Path.Combine(Path.GetTempPath(), "SpellingChecker_Update");
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
                Directory.CreateDirectory(tempDir);

                var zipPath = Path.Combine(tempDir, "update.zip");

                // Download the update
                using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    var bytesRead = 0L;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        int read;

                        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            bytesRead += read;

                            if (totalBytes > 0)
                            {
                                var percentage = (int)((bytesRead * 100) / totalBytes);
                                progress?.Report(percentage);
                            }
                        }
                    }
                }

                // Extract the ZIP file
                var extractPath = Path.Combine(tempDir, "extracted");
                ZipFile.ExtractToDirectory(zipPath, extractPath);

                // Create and run the updater script
                var currentExePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (string.IsNullOrEmpty(currentExePath))
                {
                    return false;
                }

                var currentDir = Path.GetDirectoryName(currentExePath);
                if (string.IsNullOrEmpty(currentDir))
                {
                    return false;
                }

                // Create a batch file to perform the update
                var batchPath = Path.Combine(tempDir, "update.bat");
                var batchContent = $@"@echo off
timeout /t 2 /nobreak > nul
taskkill /F /IM SpellingChecker.exe > nul 2>&1
timeout /t 1 /nobreak > nul
xcopy /E /I /Y ""{extractPath}\*"" ""{currentDir}""
timeout /t 1 /nobreak > nul
start """" ""{currentExePath}""
del ""%~f0""
";

                await File.WriteAllTextAsync(batchPath, batchContent);

                // Start the updater batch file
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = batchPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = tempDir
                };

                Process.Start(processStartInfo);

                // Exit the current application
                System.Windows.Application.Current.Shutdown();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
