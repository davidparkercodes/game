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
        
        var domainSharedPath = System.IO.Path.Combine(domainPath, "Shared", "Interfaces");
        var domainValueObjectsPath = System.IO.Path.Combine(domainPath, "ValueObjects");
        var applicationSharedPath = System.IO.Path.Combine(applicationPath, "Shared", "Cqrs");

        System.IO.Directory.Exists(srcPath).Should().BeTrue($"src directory should exist at: {srcPath}");
        System.IO.Directory.Exists(domainPath).Should().BeTrue($"Domain directory should exist at: {domainPath}");
        System.IO.Directory.Exists(applicationPath).Should().BeTrue($"Application directory should exist at: {applicationPath}");
        System.IO.Directory.Exists(domainSharedPath).Should().BeTrue($"Domain/Shared/Interfaces directory should exist at: {domainSharedPath}");
        System.IO.Directory.Exists(domainValueObjectsPath).Should().BeTrue($"Domain/ValueObjects directory should exist at: {domainValueObjectsPath}");
        System.IO.Directory.Exists(applicationSharedPath).Should().BeTrue($"Application/Shared/Cqrs directory should exist at: {applicationSharedPath}");
    }
}
