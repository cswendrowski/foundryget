using FoundryGet.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoundryGet.Models
{
    public class FoundryDataFolder
    {
        private static readonly string FoundryGetModuleRelativePath = $"modules{Path.DirectorySeparatorChar}foundryget";

        private string Folder { get; set; }

        public string ModulesFolder => $"{Folder}modules";

        public string SystemsFolder => $"{Folder}systems";

        public List<Manifest> AllManifests { get; set; } = new List<Manifest>();

        private bool ManifestsLoaded = false;

        public async Task<List<Manifest>> ReadAllManifests(IManifestLoader manifestLoader)
        {
            if (ManifestsLoaded) return AllManifests;

            await foreach (var manifest in ReadSystemManifests(manifestLoader))
            {
                AllManifests.Add(manifest);
            }

            await foreach (var manifest in ReadModuleManifests(manifestLoader))
            {
                AllManifests.Add(manifest);
            }

            ManifestsLoaded = true;
            return AllManifests;
        }

        public async IAsyncEnumerable<Manifest> ReadSystemManifests(IManifestLoader manifestLoader) {
            var manifests = Directory.GetFiles(SystemsFolder, searchPattern: "system.json", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                yield return await manifestLoader.FromFile(manifest);
            }
        }

        public async IAsyncEnumerable<Manifest> ReadModuleManifests(IManifestLoader manifestLoader)
        {
            var manifests = Directory.GetFiles(ModulesFolder, searchPattern: "module.json", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                yield return await manifestLoader.FromFile(manifest);
            }
        }


        public static FoundryDataFolder FromCurrentDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine("Currently in " + currentDirectory);
            if (currentDirectory.EndsWith(FoundryGetModuleRelativePath))
            {
                Console.WriteLine("Detected we are running from inside of a Module installation");
                currentDirectory = currentDirectory.Replace(FoundryGetModuleRelativePath, "");
            }

            return FromDirectoryPath(currentDirectory);
        }

        public static FoundryDataFolder FromDirectoryPath(string path)
        {
            if (!path.Contains("Data", StringComparison.Ordinal))
            {
                Console.WriteLine($"{path} does not appear to be the Foundry /data/ folder, exitting");
                return null;
            }

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                path += Path.DirectorySeparatorChar;

            return new FoundryDataFolder { Folder = path };
        }
    }
}
