using Game.Application.Buildings.Configuration;

namespace Game.Application.Buildings.Services;

public interface IBuildingDefaultsConfigService
{
    BuildingDefaultsConfig GetBuildingDefaultsConfig();
    int GetDefaultCost();
    int GetDefaultDamage();
    float GetDefaultRange();
    float GetDefaultAttackSpeed();
    int GetRangeCircleSegments();
    float GetRangeCircleWidth();
    ColorConfig GetRangeCircleColor();
    float GetRotationSpeed();
    float GetRotationThreshold();
    int GetMaxPooledBullets();
    int[] GetCollisionLayers();
}
