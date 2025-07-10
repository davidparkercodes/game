using Godot;
using System.Collections.Generic;

namespace Game.Infrastructure.Economy.Services;

public class GameEconomyConfigService
{
    public static GameEconomyConfigService Instance { get; private set; } = null!;
    
    private const string ConfigFilePath = "res://config/gameplay/game_economy_config.json";
    private const string LogPrefix = "💰 [ECONOMY-CONFIG]";
    
    private GameEconomyData _economyData = null!;
    
    static GameEconomyConfigService()
    {
        Instance = new GameEconomyConfigService();
    }
    
    public void Initialize()
    {
        GD.Print($"{LogPrefix} Initializing economy configuration service...");
        LoadEconomyConfig();
        GD.Print($"{LogPrefix} Economy configuration loaded successfully");
    }
    
    private void LoadEconomyConfig()
    {
        try
        {
            var configFile = FileAccess.Open(ConfigFilePath, FileAccess.ModeFlags.Read);
            if (configFile == null)
            {
                GD.PrintErr($"{LogPrefix} Could not open config file: {ConfigFilePath}");
                LoadDefaultConfig();
                return;
            }
            
            var jsonContent = configFile.GetAsText();
            configFile.Close();
            
            var json = new Json();
            var parseResult = json.Parse(jsonContent);
            
            if (parseResult != Error.Ok)
            {
                GD.PrintErr($"{LogPrefix} Error parsing config JSON: {parseResult}");
                LoadDefaultConfig();
                return;
            }
            
            var jsonData = json.Data.AsGodotDictionary();
            _economyData = ParseEconomyData(jsonData);
            
            GD.Print($"{LogPrefix} Successfully loaded economy configuration");
            GD.Print($"{LogPrefix} Starting Money: {_economyData.StartingMoney}, Lives: {_economyData.StartingLives}, Score: {_economyData.StartingScore}");
            GD.Print($"{LogPrefix} Score Multiplier: {_economyData.KillScoreMultiplier}, Sell Percentage: {_economyData.SellPercentage}");
            GD.Print($"{LogPrefix} Max Upgrade Levels: {_economyData.MaxUpgradeLevels}, Upgrade Cost Multiplier: {_economyData.UpgradeCostMultiplier}");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Exception loading config: {ex.Message}");
            LoadDefaultConfig();
        }
    }
    
    private GameEconomyData ParseEconomyData(Godot.Collections.Dictionary jsonData)
    {
        var economyData = new GameEconomyData();
        
        // Parse starting values
        if (jsonData.ContainsKey("starting_values"))
        {
            var startingValues = jsonData["starting_values"].AsGodotDictionary();
            economyData.StartingMoney = startingValues["money"].AsInt32();
            economyData.StartingLives = startingValues["lives"].AsInt32();
            economyData.StartingScore = startingValues["score"].AsInt32();
            GD.Print($"{LogPrefix} Parsed starting values: Money={economyData.StartingMoney}, Lives={economyData.StartingLives}, Score={economyData.StartingScore}");
        }
        else
        {
            GD.PrintErr($"{LogPrefix} 'starting_values' section not found in config file!");
        }
        
        // Parse score system
        if (jsonData.ContainsKey("score_system"))
        {
            var scoreSystem = jsonData["score_system"].AsGodotDictionary();
            economyData.KillScoreMultiplier = scoreSystem["kill_score_multiplier"].AsInt32();
        }
        
        // Parse selling system
        if (jsonData.ContainsKey("selling"))
        {
            var selling = jsonData["selling"].AsGodotDictionary();
            economyData.SellPercentage = selling["sell_percentage"].AsSingle();
        }
        
        // Parse upgrade system
        if (jsonData.ContainsKey("upgrade_system"))
        {
            var upgradeSystem = jsonData["upgrade_system"].AsGodotDictionary();
            economyData.MaxUpgradeLevels = upgradeSystem["max_upgrade_levels"].AsInt32();
            economyData.UpgradeCostMultiplier = upgradeSystem["upgrade_cost_multiplier"].AsSingle();
            economyData.UpgradeDamageMultiplier = upgradeSystem["upgrade_damage_multiplier"].AsSingle();
            economyData.UpgradeRangeMultiplier = upgradeSystem["upgrade_range_multiplier"].AsSingle();
        }
        
        return economyData;
    }
    
    private void LoadDefaultConfig()
    {
        GD.Print($"{LogPrefix} Loading default economy configuration");
        _economyData = new GameEconomyData
        {
            StartingMoney = 500,
            StartingLives = 20,
            StartingScore = 0,
            KillScoreMultiplier = 10,
            SellPercentage = 0.75f,
            MaxUpgradeLevels = 3,
            UpgradeCostMultiplier = 1.5f,
            UpgradeDamageMultiplier = 1.3f,
            UpgradeRangeMultiplier = 1.1f
        };
    }
    
    // Public accessors
    public int GetStartingMoney() 
    {
        if (_economyData == null)
        {
            GD.PrintErr($"{LogPrefix} Economy data not loaded! Using fallback starting money: 500");
            return 500;
        }
        return _economyData.StartingMoney;
    }
    
    public int GetStartingLives() 
    {
        if (_economyData == null)
        {
            GD.PrintErr($"{LogPrefix} Economy data not loaded! Using fallback starting lives: 20");
            return 20;
        }
        return _economyData.StartingLives;
    }
    
    public int GetStartingScore() 
    {
        if (_economyData == null)
        {
            GD.PrintErr($"{LogPrefix} Economy data not loaded! Using fallback starting score: 0");
            return 0;
        }
        return _economyData.StartingScore;
    }
    
    public int GetKillScoreMultiplier() => _economyData?.KillScoreMultiplier ?? 10;
    public float GetSellPercentage() => _economyData?.SellPercentage ?? 0.75f;
    public int GetMaxUpgradeLevels() => _economyData?.MaxUpgradeLevels ?? 3;
    public float GetUpgradeCostMultiplier() => _economyData?.UpgradeCostMultiplier ?? 1.5f;
    public float GetUpgradeDamageMultiplier() => _economyData?.UpgradeDamageMultiplier ?? 1.3f;
    public float GetUpgradeRangeMultiplier() => _economyData?.UpgradeRangeMultiplier ?? 1.1f;
    
    public GameEconomyData GetEconomyData() => _economyData ?? new GameEconomyData();
}

public class GameEconomyData
{
    public int StartingMoney { get; set; } = 500;
    public int StartingLives { get; set; } = 20;
    public int StartingScore { get; set; } = 0;
    public int KillScoreMultiplier { get; set; } = 10;
    public float SellPercentage { get; set; } = 0.75f;
    public int MaxUpgradeLevels { get; set; } = 3;
    public float UpgradeCostMultiplier { get; set; } = 1.5f;
    public float UpgradeDamageMultiplier { get; set; } = 1.3f;
    public float UpgradeRangeMultiplier { get; set; } = 1.1f;
}
