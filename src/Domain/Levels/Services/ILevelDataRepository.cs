using Game.Domain.Levels.ValueObjects;
using System.Threading.Tasks;

namespace Game.Domain.Levels.Services;

public interface ILevelDataRepository
{
    Task<LevelData?> LoadLevelAsync(string levelName);
    Task<bool> SaveLevelAsync(string levelName, LevelData levelData);
    Task<string[]> GetAvailableLevelsAsync();
    Task<bool> LevelExistsAsync(string levelName);
    LevelData GetDefaultLevel();
}
