using FluentAssertions;
using Game.Domain.Audio.Enums;
using Game.Domain.Audio.ValueObjects;
using Game.Domain.Shared.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Audio.ValueObjects;

public class SoundRequestTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSoundRequest()
    {
        var soundRequest = new SoundRequest("explosion_sound", SoundCategory.SFX, -5.0f);

        soundRequest.SoundKey.Should().Be("explosion_sound");
        soundRequest.Category.Should().Be(SoundCategory.SFX);
        soundRequest.VolumeDb.Should().Be(-5.0f);
        soundRequest.MaxDistance.Should().Be(500.0f);
    }

    [Fact]
    public void IsPositional_WithBothPositions_ShouldReturnTrue()
    {
        var position = new Position(100.0f, 200.0f);
        var listenerPosition = new Position(150.0f, 250.0f);
        var soundRequest = new SoundRequest("test_sound", position: position, listenerPosition: listenerPosition);

        soundRequest.IsPositional.Should().BeTrue();
    }

    [Fact]
    public void IsPositional_WithMissingPosition_ShouldReturnFalse()
    {
        var listenerPosition = new Position(150.0f, 250.0f);
        var soundRequest = new SoundRequest("test_sound", listenerPosition: listenerPosition);

        soundRequest.IsPositional.Should().BeFalse();
    }
}
