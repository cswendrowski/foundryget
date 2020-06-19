using FoundryGet.Interfaces;
using Moq;
using NuGet.Versioning;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FoundryGet.Models.Tests
{
    public class DependencyChainTests
    {
       
        private IManifestLoader CreateMockManifestLoader(params Manifest[] manifests)
        {
            var mockManifestLoader = new Mock<IManifestLoader>();
            foreach (var manifest in manifests)
            {
                mockManifestLoader.Setup(x => x.FromUri(manifest.ManifestUri)).ReturnsAsync(manifest);
            }
            return mockManifestLoader.Object;
        }

        private Manifest CreateTestManifest(string name, int major, int minor, int patch, params Manifest[] dependencies)
        {
            var manifest = new Manifest()
            {
                Name = name,
                SemanticVersion = new SemanticVersion(major, minor, patch),
                ManifestUri = new Uri("http://fake.com/" + name)
            };

            if (dependencies != null && dependencies.Any())
            {
                manifest.Dependencies = dependencies.Select(x => x.ToDependency()).ToList();
            }

            return manifest;
        }

        [Fact]
        public async Task GivenInstallationManifestWithOneDependency_WhenChainCalculated_ThenOneDependency()
        {
            var dependencyManifest = CreateTestManifest("dependency-a", 1, 0, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest);

            var installManifest = CreateTestManifest("to-install", 1, 0, 0, dependencyManifest);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifest);

            dependencyChain.NeededDependencies.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GivenTwoManifestWithDependencies_WhenChainCalculated_ThenTwoDependencies()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-b", 1, 0, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest1);
            var installManifestB = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest2);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);

            dependencyChain.NeededDependencies.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GivenTwoManifestWithSameDependency_WhenChainCalculated_ThenOneDependency()
        {
            var dependencyManifest = CreateTestManifest("dependency-a", 1, 0, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest);
            var installManifestB = CreateTestManifest("to-install-b", 1, 0, 0, dependencyManifest);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);

            dependencyChain.NeededDependencies.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GivenSecondManifestWithCompatibleDependencyLoaded_WhenChainCalculated_ThenOneDependency()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-a", 1, 2, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest1);
            var installManifestB = CreateTestManifest("to-install-b", 1, 0, 0, dependencyManifest2);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);

            dependencyChain.NeededDependencies.Count.ShouldBe(1);
            dependencyChain.NeededDependencies.First().Version.ShouldBe(dependencyManifest2.SemanticVersion);
        }

        [Fact]
        public async Task GivenTwoManifestWithCompatibleDependency_WhenChainCalculated_ThenOneDependency()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-a", 1, 2, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest1);
            var installManifestB = CreateTestManifest("to-install-b", 1, 0, 0, dependencyManifest2);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);

            dependencyChain.NeededDependencies.Count.ShouldBe(1);
            dependencyChain.NeededDependencies.First().Version.ShouldBe(dependencyManifest2.SemanticVersion);
        }

        [Fact]
        public async Task GivenTwoManifestWithIncompatibleDependency_WhenChainCalculated_ThenIncompatible()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-a", 2, 0, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest1);
            var installManifestB = CreateTestManifest("to-install-b", 1, 0, 0, dependencyManifest2);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);

            var exception = await Should.ThrowAsync<Exception>(async () =>
            {
                await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);
            });

            exception.Message.ShouldBe("Incompatible dependency chain");
        }

        [Fact]
        public async Task GivenManifestsWithSharedDownstreamDependency_WhenChainCalculated_ThenSharedLoadedOnce()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-b", 1, 0, 0);
            var dependencyManifest3 = CreateTestManifest("dependency-c", 1, 0, 0, dependencyManifest1);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2, dependencyManifest3);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest1);
            var installManifestB = CreateTestManifest("to-install-b", 1, 0, 0, dependencyManifest2, dependencyManifest3);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestB);

            dependencyChain.NeededDependencies.Count.ShouldBe(3);
            dependencyChain.NeededDependencies.First().Version.ShouldBe(dependencyManifest2.SemanticVersion);
        }

        [Fact]
        public async Task GivenCircularDependency_WhenChainCalculated_ThenResolved()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 0, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-b", 1, 0, 0, dependencyManifest1);
            var dependencyManifest3 = CreateTestManifest("dependency-c", 1, 0, 0, dependencyManifest2);
            dependencyManifest1.Dependencies = new List<Dependency> { dependencyManifest3.ToDependency() };

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2, dependencyManifest3);

            var installManifestA = CreateTestManifest("to-install-a", 1, 0, 0, dependencyManifest3);

            var dependencyChain = new DependencyChain();
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifestA);

            dependencyChain.NeededDependencies.Count.ShouldBe(3);
            dependencyChain.NeededDependencies.First().Version.ShouldBe(dependencyManifest2.SemanticVersion);
        }

        [Fact]
        public async Task GivenAlreadyInstalledDependency_WhenChainCalculated_ThenOneDependency()
        {
            var dependencyManifest1 = CreateTestManifest("dependency-a", 1, 2, 0);
            var dependencyManifest2 = CreateTestManifest("dependency-a", 1, 0, 0);

            var manifestLoader = CreateMockManifestLoader(dependencyManifest1, dependencyManifest2);

            var alreadyInstalledManifest = CreateTestManifest("installed", 1, 0, 0, dependencyManifest1);
            var installManifest = CreateTestManifest("to-install", 1, 0, 0, dependencyManifest2);

            var dependencyChain = new DependencyChain();

            await dependencyChain.AddDependenciesFromManifest(manifestLoader, alreadyInstalledManifest);
            await dependencyChain.AddDependenciesFromManifest(manifestLoader, installManifest);

            dependencyChain.NeededDependencies.Count.ShouldBe(1);
            dependencyChain.NeededDependencies.First().Version.ShouldBe(dependencyManifest1.GetSemanticVersion());
        }
    }
}
