using Xunit;
using FluentAssertions;

namespace Game.Tests.Domain;

public class TestSetupTests
{
    [Fact]
    public void TestFramework_ShouldBeConfiguredCorrectly()
    {
        var result = 2 + 2;
        result.Should().Be(4);
    }

    [Fact]
    public void CleanArchitecture_DirectoryStructure_ShouldExist()
    {
        var currentDirectory = System.IO.Directory.GetCurrentDirectory();
        var projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, "..", "..", "..", "..", ".."));
        var srcPath = System.IO.Path.Combine(projectRoot, "src");
        var domainPath = System.IO.Path.Combine(srcPath, "Domain");
        var applicationPath = System.IO.Path.Combine(srcPath, "Application");
        
        // Feature-centric domain structure
        var domainBuildingsServicesPath = System.IO.Path.Combine(domainPath, "Buildings", "Services");
        var domainEnemiesServicesPath = System.IO.Path.Combine(domainPath, "Enemies", "Services");
        var applicationSharedPath = System.IO.Path.Combine(applicationPath, "Shared", "Cqrs");
        
        // Feature-centric application structure
        var applicationBuildingsPath = System.IO.Path.Combine(applicationPath, "Buildings");
        var applicationGamePath = System.IO.Path.Combine(applicationPath, "Game");

        System.IO.Directory.Exists(srcPath).Should().BeTrue($"src directory should exist at: {srcPath}");
        System.IO.Directory.Exists(domainPath).Should().BeTrue($"Domain directory should exist at: {domainPath}");
        System.IO.Directory.Exists(applicationPath).Should().BeTrue($"Application directory should exist at: {applicationPath}");
        System.IO.Directory.Exists(domainBuildingsServicesPath).Should().BeTrue($"Domain/Buildings/Services directory should exist at: {domainBuildingsServicesPath}");
        System.IO.Directory.Exists(domainEnemiesServicesPath).Should().BeTrue($"Domain/Enemies/Services directory should exist at: {domainEnemiesServicesPath}");
        System.IO.Directory.Exists(applicationSharedPath).Should().BeTrue($"Application/Shared/Cqrs directory should exist at: {applicationSharedPath}");
        System.IO.Directory.Exists(applicationBuildingsPath).Should().BeTrue($"Application/Buildings directory should exist at: {applicationBuildingsPath}");
        System.IO.Directory.Exists(applicationGamePath).Should().BeTrue($"Application/Game directory should exist at: {applicationGamePath}");
    }
}
