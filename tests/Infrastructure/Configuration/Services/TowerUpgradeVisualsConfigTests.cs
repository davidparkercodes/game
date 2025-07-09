using FluentAssertions;
using Game.Infrastructure.Configuration.Services;
using Xunit;

namespace Game.Tests.Infrastructure.Configuration.Services;

public class TowerUpgradeVisualsConfigTests
{
    [Fact]
    public void GetScaleForLevel_WithLevel0_ShouldReturn1()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var scale = config.GetScaleForLevel(0);
        
        scale.Should().Be(1.0f);
    }

    [Fact]
    public void GetScaleForLevel_WithLevel1_ShouldReturn110Percent()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var scale = config.GetScaleForLevel(1);
        
        scale.Should().Be(1.1f);
    }

    [Fact]
    public void GetScaleForLevel_WithLevel2_ShouldReturn120Percent()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var scale = config.GetScaleForLevel(2);
        
        scale.Should().Be(1.2f);
    }

    [Fact]
    public void GetScaleForLevel_WithLevel3_ShouldReturn130Percent()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var scale = config.GetScaleForLevel(3);
        
        scale.Should().Be(1.3f);
    }

    [Fact]
    public void GetColorForLevel_WithLevel0_ShouldReturnWhite()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var color = config.GetColorForLevel(0);
        
        color.R.Should().Be(1.0f);
        color.G.Should().Be(1.0f);
        color.B.Should().Be(1.0f);
        color.A.Should().Be(1.0f);
    }

    [Fact]
    public void GetColorForLevel_WithLevel1_ShouldReturnRedTint()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var color = config.GetColorForLevel(1);
        
        color.R.Should().Be(1.1f);
        color.G.Should().Be(1.0f);
        color.B.Should().Be(1.0f);
        color.A.Should().Be(1.0f);
    }

    [Fact]
    public void GetColorForLevel_WithLevel2_ShouldReturnGreenTint()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var color = config.GetColorForLevel(2);
        
        color.R.Should().Be(1.0f);
        color.G.Should().Be(1.1f);
        color.B.Should().Be(1.0f);
        color.A.Should().Be(1.0f);
    }

    [Fact]
    public void GetColorForLevel_WithLevel3_ShouldReturnBlueTint()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var color = config.GetColorForLevel(3);
        
        color.R.Should().Be(1.0f);
        color.G.Should().Be(1.0f);
        color.B.Should().Be(1.1f);
        color.A.Should().Be(1.0f);
    }

    [Fact]
    public void GetColorForLevel_WithLevel4_ShouldReturnGoldenTint()
    {
        var config = TowerUpgradeVisualsConfig.Instance;
        
        var color = config.GetColorForLevel(4);
        
        color.R.Should().Be(1.2f);
        color.G.Should().Be(1.2f);
        color.B.Should().Be(1.0f);
        color.A.Should().Be(1.0f);
    }
}
