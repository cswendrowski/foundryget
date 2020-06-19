using FoundryGet.Models;
using Newtonsoft.Json;
using NuGet.Versioning;
using Shouldly;
using Xunit;

namespace FoundryGet.Models.Tests
{
    public class DependencyTests
    {
        [Theory]

        [InlineData("1.0.0", "1.1.0", true)]
        [InlineData("1.0.0", "1.0.1", true)]
        [InlineData("1.0.5", "1.1.0", true)]

        [InlineData("1.0.0", "2.0.0", false)]
        [InlineData("1.0.0", "0.1.0", false)]
        [InlineData("1.5.0", "1.1.0", false)]
        public void IsSatisfiedByTest(string dependencyVersion, string installedVersion, bool shouldBeSatisfied)
        {
            var dependency = new Dependency()
            {
                Name = "test-package",
                Version = SemanticVersion.Parse(dependencyVersion)
            };

            var manifest = new Manifest
            {
                Name = "test-package",
                SemanticVersion = SemanticVersion.Parse(installedVersion)
            };

            dependency.IsSatisfiedBy(manifest).ShouldBe(shouldBeSatisfied);
        }

        [Theory]

        [InlineData("1.0.0", "1.1.0", true)]
        [InlineData("1.0.0", "1.0.1", true)]
        [InlineData("1.0.5", "1.1.0", true)]

        [InlineData("1.0.0", "2.0.0", false)]
        [InlineData("1.0.0", "0.1.0", false)]
        [InlineData("1.5.0", "1.1.0", false)]
        public void IsSatisfiedByTest1(string dependencyVersion, string installedVersion, bool shouldBeSatisfied)
        {
            var dependency = new Dependency()
            {
                Name = "test-package",
                Version = SemanticVersion.Parse(dependencyVersion)
            };

            var dependency2 = new Dependency()
            {
                Name = "test-package",
                Version = SemanticVersion.Parse(installedVersion)
            };

            dependency.IsSatisfiedBy(dependency2).ShouldBe(shouldBeSatisfied);
        }

        [Fact]
        public void CanConvertJsonToDependency()
        {
            var json = "{ \"name\": \"archmage\", \"manifest\": \"https://gitlab.com/asacolips-projects/foundry-mods/archmage/-/raw/1.5.0/system.json\", \"version\": \"1.5.0\" }";

            Should.NotThrow(() =>
            {
                var dependency = JsonConvert.DeserializeObject<Dependency>(json);

                dependency.ShouldNotBeNull();
                dependency.Name.ShouldBe("archmage");
                dependency.ManifestUri.ShouldBe(new System.Uri("https://gitlab.com/asacolips-projects/foundry-mods/archmage/-/raw/1.5.0/system.json"));
                dependency.Version.ShouldBe(new SemanticVersion(1, 5, 0));
            });
        }
    }
}
