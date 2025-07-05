using Xunit;
using FluentAssertions;

namespace Game.Tests.Domain
{
    /// <summary>
    /// Basic test to verify our testing infrastructure is set up correctly
    /// </summary>
    public class TestSetupTests
    {
        [Fact]
        public void TestFramework_ShouldBeConfiguredCorrectly()
        {
            // Arrange & Act
            var result = 2 + 2;

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void Domain_DirectoryStructure_ShouldExist()
        {
            // Arrange & Act
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            var domainPath = System.IO.Path.Combine(currentDirectory, "src", "Domain");
            var entitiesPath = System.IO.Path.Combine(domainPath, "Entities");
            var valueObjectsPath = System.IO.Path.Combine(domainPath, "ValueObjects");
            var servicesPath = System.IO.Path.Combine(domainPath, "Services");

            // Assert
            System.IO.Directory.Exists(domainPath).Should().BeTrue("Domain directory should exist");
            System.IO.Directory.Exists(entitiesPath).Should().BeTrue("Entities directory should exist");
            System.IO.Directory.Exists(valueObjectsPath).Should().BeTrue("ValueObjects directory should exist");
            System.IO.Directory.Exists(servicesPath).Should().BeTrue("Services directory should exist");
        }
    }
}
