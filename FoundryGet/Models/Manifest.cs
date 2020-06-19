using FoundryGet.Interfaces;
using Newtonsoft.Json;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FoundryGet.Models
{
    public partial class Manifest
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("semanticVersion", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SemanticVersionConverter))]
        public SemanticVersion SemanticVersion { get; set; }

        [JsonProperty("dependencies", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dependency> Dependencies { get; set; } = new List<Dependency>();

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("minimumCoreVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string MinimumCoreVersion { get; set; }

        [JsonProperty("compatibleCoreVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string CompatibleCoreVersion { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string Author { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ProjectUrl { get; set; }

        [JsonProperty("manifest", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ManifestUri { get; set; }

        [JsonProperty("download", NullValueHandling = NullValueHandling.Ignore)]
        public Uri DownloadUri { get; set; }

        public bool IsSystem => ManifestUri.AbsoluteUri.EndsWith("system.json", StringComparison.OrdinalIgnoreCase);

        public bool IsModule => ManifestUri.AbsoluteUri.EndsWith("module.json", StringComparison.OrdinalIgnoreCase);
    }

    public partial class Manifest
    {
        private HttpClient _httpClient = new HttpClient();

        public SemanticVersion GetSemanticVersion()
        {
            if (SemanticVersion != null) return SemanticVersion;

            if (SemanticVersion.TryParse(Version, out var semanticVersion))
            {
                return semanticVersion;
            }

            Console.WriteLine($"{Version} did not appear to be a proper Semantic Version. Either ask the Module / System developer to update the 'version' or 'semanticVersion' field with a valid SemVer. Read more here: https://semver.org/");
            return null;
        }

        public Dependency ToDependency()
        {
            return new Dependency
            {
                Name = Name,
                Version = GetSemanticVersion(),
                ManifestUri = ManifestUri
            };
        }

        public async Task<int> Install(FoundryDataFolder dataFolder)
        {
            if (IsModule && IsSystem)
            {
                Console.WriteLine("Detected this Manifest as both a System and a Module, which is impossible");
                return 1;
            }

            Console.WriteLine($"Installing {Name} version {GetSemanticVersion().ToNormalizedString()} from {DownloadUri}");

            var stream = await _httpClient.GetStreamAsync(DownloadUri);
            var zip = new ZipArchive(stream);

            var installationDestination = GetDestinationPath(dataFolder);

            var manifestRelativePath = string.Empty;
            if (IsModule)
            {
                var moduleJson = zip.Entries.FirstOrDefault(x => x.Name == "module.json");
                if (moduleJson == null)
                {
                    Console.WriteLine("Could not find module.json in the downloaded ZIP");
                    return 1;
                }
                manifestRelativePath = moduleJson.FullName.Replace("module.json", "");
            }

            if (IsSystem)
            {
                var systemJson = zip.Entries.FirstOrDefault(x => x.Name == "system.json");
                if (systemJson == null)
                {
                    Console.WriteLine("Could not find system.json in the downloaded ZIP");
                    return 1;
                }
                manifestRelativePath = systemJson.FullName.Replace("system.json", "");
            }

            Console.WriteLine("Extracting ZIP to " + installationDestination);
            Directory.CreateDirectory(installationDestination);
            ExtractZipToDestination(zip, installationDestination, manifestRelativePath);

            return 0;
        }

        private static void ExtractZipToDestination(ZipArchive zip, string installationDestination, string manifestRelativePath)
        {
            foreach (var entry in zip.Entries)
            {
                // Gets the full path to ensure that relative segments are removed.
                var entryName = entry.FullName;

                if (!string.IsNullOrEmpty(manifestRelativePath))
                {
                    entryName = entryName.Replace(manifestRelativePath, "");
                }

                string fileDestinationPath = Path.GetFullPath(Path.Combine(installationDestination, entryName));

                if (fileDestinationPath.Equals(installationDestination, StringComparison.Ordinal)) continue;

                // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                // are case-insensitive.
                if (fileDestinationPath.StartsWith(installationDestination, StringComparison.Ordinal))
                {
                    if (fileDestinationPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    {
                        Directory.CreateDirectory(fileDestinationPath);
                    }
                    else
                    {
                        entry.ExtractToFile(fileDestinationPath, true);
                    }
                }
            }
        }

        private string GetDestinationPath(FoundryDataFolder dataFolder)
        {
            if (IsModule)
            {
                return dataFolder.ModulesFolder + Path.DirectorySeparatorChar + Name + Path.DirectorySeparatorChar;
            }
            
            if (IsSystem)
            {
                return dataFolder.SystemsFolder + Path.DirectorySeparatorChar + Name + Path.DirectorySeparatorChar;
            }

            return null;
        }
    }
}
