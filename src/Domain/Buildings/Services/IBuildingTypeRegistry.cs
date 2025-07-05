using System.Collections.Generic;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Services;

public interface IBuildingTypeRegistry
{
    BuildingType? GetByInternalId(string internalId);
    BuildingType? GetByConfigKey(string configKey);
    IEnumerable<BuildingType> GetByCategory(string category);
    BuildingType? GetDefaultType();
    BuildingType? GetCheapestType();
    bool IsValidConfigKey(string configKey);
    bool IsValidInternalId(string internalId);
    IEnumerable<BuildingType> GetAllTypes();
    IEnumerable<string> GetAllCategories();
    IEnumerable<BuildingType> GetAllByTier(int tier);
}
