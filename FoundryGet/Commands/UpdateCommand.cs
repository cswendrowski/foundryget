using FoundryGet.Interfaces;
using FoundryGet.Models;
using FoundryGet.Services;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoundryGet.Commands
{
    public static class UpdateCommand
    {
        public static void Command(CommandLineApplication command)
        {
            command.Description = "Update modules";
            command.HelpOption("-?|-h|--help");

            //var url = command.Argument("[url]", "The manifest URL of the module");
            var dataFolder = command.Option("-d|--dataFolder", "The Foundry Data folder to install in, such as C:\\Users\\Me\\AppData\\Local\\FoundryVTT\\Data", CommandOptionType.SingleValue);

            command.OnExecute(async () => await Execute(dataFolder));
        }

        private static async Task<int> Execute(CommandOption dataFolder)
        {
            try
            {
                FoundryDataFolder foundryDataFolder;
                if (dataFolder.HasValue())
                {
                    foundryDataFolder = FoundryDataFolder.FromDirectoryPath(dataFolder.Value());
                }
                else
                {
                    foundryDataFolder = FoundryDataFolder.FromCurrentDirectory();
                }

                if (foundryDataFolder == null) return 1;

                var manifestLoader = new ManifestLoader();
                var manifestsToUpdate = new List<Manifest>();

                foreach (var manifest in await foundryDataFolder.ReadAllManifests(manifestLoader))
                {
                    Console.WriteLine();
                    Console.WriteLine($"Checking {manifest.Name} for updates");
                    var latestManifest = await GetLatestManifest(manifestLoader, manifest);
                    if (latestManifest == null) continue;

                    if (latestManifest.GetSemanticVersion() > manifest.GetSemanticVersion())
                    {
                        Console.WriteLine($"Queuing update for {latestManifest.Name} from {manifest.GetSemanticVersion().ToNormalizedString()} to {latestManifest.GetSemanticVersion().ToNormalizedString()}");
                        manifestsToUpdate.Add(latestManifest);
                    }
                }
                
                var dependencyChain = new DependencyChain();
                dependencyChain.AddCurrentlyInstalledDependencies(manifestLoader, foundryDataFolder);

                foreach (var toUpdate in manifestsToUpdate)
                {
                    await dependencyChain.AddDependenciesFromManifest(manifestLoader, toUpdate);
                }

                foreach (var dependency in dependencyChain.NeededDependencies)
                {
                    Console.WriteLine();
                    var dependencyManifest = await dependency.GetFullManifest(manifestLoader);
                    await dependencyManifest.Install(foundryDataFolder);
                }

                foreach (var toUpdate in manifestsToUpdate)
                {
                    Console.WriteLine();
                    await toUpdate.Install(foundryDataFolder);
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }


        private static async Task<Manifest> GetLatestManifest(IManifestLoader manifestLoader, Manifest manifest)
        {
            var version = manifest.GetSemanticVersion();
            if (version == null)
            {
                Console.WriteLine("Could not get proper SemanticVersion for " + manifest.Name);
                return null;
            }

            return await manifestLoader.FromUri(manifest.ManifestUri);
        }
    }

}
