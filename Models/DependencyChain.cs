using FoundryGet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoundryGet.Models
{
    public class DependencyChain
    {
        public HashSet<Dependency> InstalledDependencies { get; set; } = new HashSet<Dependency>();
        public HashSet<Dependency> NeededDependencies { get; set; } = new HashSet<Dependency>();

        public DependencyChain AddCurrentlyInstalledDependencies(IManifestLoader manifestLoader, FoundryDataFolder dataFolder)
        {
            foreach (var manifest in dataFolder.AllManifests)
            {
                InstalledDependencies.Add(manifest.ToDependency());
            }
            return this;
        }

        public async Task<DependencyChain> AddDependenciesFromManifest(IManifestLoader manifestLoader, Manifest manifest)
        {
            foreach (var dependency in manifest.Dependencies.Where(dependency => !NeededDependencies.Any(current => dependency.IsSatisfiedBy(current) && !InstalledDependencies.Any(current => dependency.IsSatisfiedBy(current)))))
            {
                Console.WriteLine("Calculating dependencies for " + manifest.Name);
                
                var existingInstalledDependency = InstalledDependencies.FirstOrDefault(x => x.Name == dependency.Name);
                var existingNeededDependency = NeededDependencies.FirstOrDefault(x => x.Name == dependency.Name);

                if (existingNeededDependency != null)
                {
                    if (existingNeededDependency.Version.Major != dependency.Version.Major)
                    {
                        Console.WriteLine($"Dependency on {dependency.Name} version {dependency.Version.ToNormalizedString()} is incompatible with dependency on Version {existingNeededDependency.Version.ToNormalizedString()}");
                        throw new Exception("Incompatible dependency chain");
                    }
                    if (existingNeededDependency.Version > dependency.Version)
                    {
                        Console.WriteLine($"Dependency on {dependency.Name} version {dependency.Version.ToNormalizedString()} is already satisfied by Version {existingNeededDependency.Version.ToNormalizedString()}");
                    }
                    else
                    {
                        Console.WriteLine($"Upgrading dependency on {dependency.Name} version {existingNeededDependency.Version.ToNormalizedString()} to Version {dependency.Version.ToNormalizedString()}");
                        NeededDependencies.Remove(existingNeededDependency);
                        NeededDependencies.Add(dependency);
                    }
                }
                else if (existingInstalledDependency != null)
                {
                    if (existingInstalledDependency.Version.Major != dependency.Version.Major)
                    {
                        Console.WriteLine($"Dependency on {dependency.Name} version {dependency.Version.ToNormalizedString()} is incompatible with dependency on Version {existingInstalledDependency.Version.ToNormalizedString()}");
                        throw new Exception("Incompatible dependency chain");
                    }
                    if (existingInstalledDependency.Version > dependency.Version)
                    {
                        Console.WriteLine($"Dependency on {dependency.Name} version {dependency.Version.ToNormalizedString()} is already satisfied by Version {existingInstalledDependency.Version.ToNormalizedString()}");
                    }
                    else
                    {
                        Console.WriteLine($"Upgrading dependency on {dependency.Name} version {existingInstalledDependency.Version.ToNormalizedString()} to Version {dependency.Version.ToNormalizedString()}");
                        InstalledDependencies.Remove(existingInstalledDependency);
                        NeededDependencies.Add(dependency);
                    }
                }
                else {
                    NeededDependencies.Add(dependency);
                }
                
                var dependencyManifest = await manifestLoader.FromUri(dependency.ManifestUri);
                await AddDependenciesFromManifest(manifestLoader, dependencyManifest);
            }

            return this;
        }
    }
}
