using System.CommandLine;
using Game.Application.Simulation;
using Game.Application.Simulation.ValueObjects;
using Spectre.Console;

namespace GameSimRunner.Console;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("GameSimRunner - Tower Defense Balance Testing Tool");

        var scenarioOption = new Option<string>(
            "--scenario",
            "Run a predefined scenario or load from file path"
        );

        var verboseOption = new Option<bool>(
            "--verbose",
            "Enable verbose output with detailed simulation data"
        );

        var minimalOption = new Option<bool>(
            "--minimal",
            "Enable minimal output (overrides verbose)"
        );

        var exportJsonOption = new Option<string?>(
            "--export-json",
            "Export results to JSON file"
        );

        var compareOption = new Option<string[]>(
            "--compare",
            "Compare two config files (baseline vs modified)"
        ) { AllowMultipleArgumentsPerToken = true };

        rootCommand.AddOption(scenarioOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(minimalOption);
        rootCommand.AddOption(exportJsonOption);
        rootCommand.AddOption(compareOption);

        rootCommand.SetHandler(async (scenario, verbose, minimal, exportJson, compare) =>
        {
            try
            {
                var outputLevel = minimal ? OutputLevel.Minimal : 
                                 verbose ? OutputLevel.Verbose : 
                                 OutputLevel.Normal;

                if (compare?.Length == 2)
                {
                    await RunComparison(compare[0], compare[1], outputLevel, exportJson);
                }
                else
                {
                    await RunSimulation(scenario, outputLevel, exportJson);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
            return 0;
        }, scenarioOption, verboseOption, minimalOption, exportJsonOption, compareOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task RunSimulation(string? scenario, OutputLevel outputLevel, string? exportJson)
    {
        AnsiConsole.MarkupLine("[bold cyan]🎯 GameSimRunner - Tower Defense Balance Testing[/]");
        AnsiConsole.WriteLine();

        var configPath = GetScenarioPath(scenario);
        var runner = CreateGameSimRunner();
        
        var config = await LoadConfig(configPath);
        
        if (outputLevel >= OutputLevel.Normal)
        {
            DisplayScenarioInfo(config, configPath);
        }

        var result = await RunWithProgressBar(runner, config, outputLevel);

        DisplayResults(result, outputLevel);

        if (!string.IsNullOrEmpty(exportJson))
        {
            await ExportToJson(result, exportJson);
        }
    }

    private static async Task RunComparison(string baselinePath, string modifiedPath, OutputLevel outputLevel, string? exportJson)
    {
        AnsiConsole.MarkupLine("[bold yellow]⚖️  Config Comparison Mode[/]");
        AnsiConsole.WriteLine();

        var runner = CreateGameSimRunner();
        var baselineConfig = await LoadConfig(baselinePath);
        var modifiedConfig = await LoadConfig(modifiedPath);

        AnsiConsole.MarkupLine($"[cyan]Baseline:[/] {baselinePath}");
        AnsiConsole.MarkupLine($"[cyan]Modified:[/] {modifiedPath}");
        AnsiConsole.WriteLine();

        var baselineResult = await RunWithProgressBar(runner, baselineConfig, outputLevel, "Baseline");
        var modifiedResult = await RunWithProgressBar(runner, modifiedConfig, outputLevel, "Modified");

        DisplayComparison(baselineResult, modifiedResult, outputLevel);

        if (!string.IsNullOrEmpty(exportJson))
        {
            var comparison = new { Baseline = baselineResult, Modified = modifiedResult };
            await ExportToJson(comparison, exportJson);
        }
    }

    private static GameSimRunner CreateGameSimRunner()
    {
        return new GameSimRunner();
    }

    private static string GetScenarioPath(string? scenario)
    {
        if (string.IsNullOrEmpty(scenario))
        {
            return "data/simulation/scenarios/quick-balance.json";
        }

        if (File.Exists(scenario))
        {
            return scenario;
        }

        var predefinedPath = $"data/simulation/scenarios/{scenario}.json";
        if (File.Exists(predefinedPath))
        {
            return predefinedPath;
        }

        throw new FileNotFoundException($"Scenario not found: {scenario}");
    }

    private static async Task<SimulationConfig> LoadConfig(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Config file not found: {configPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(configPath);
        var config = System.Text.Json.JsonSerializer.Deserialize<SimulationConfig>(jsonContent);
        
        if (config == null)
        {
            throw new InvalidOperationException($"Failed to deserialize config from: {configPath}");
        }

        return config;
    }

    private static void DisplayScenarioInfo(SimulationConfig config, string configPath)
    {
        var panel = new Panel($"[yellow]Scenario:[/] {Path.GetFileName(configPath)}\n" +
                             $"[yellow]Max Waves:[/] {config.MaxWaves}\n" +
                             $"[yellow]Starting Money:[/] {config.StartingMoney}\n" +
                             $"[yellow]Starting Lives:[/] {config.StartingLives}")
        {
            Header = new PanelHeader("📋 Simulation Configuration"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private static async Task<SimulationResult> RunWithProgressBar(GameSimRunner runner, SimulationConfig config, OutputLevel outputLevel, string? label = null)
    {
        var taskName = label ?? "Simulation";
        
        return await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"[cyan]{taskName}[/]", maxValue: config.MaxWaves);
                
                var progress = new Progress<SimulationProgress>(update =>
                {
                    task.Value = update.CurrentWave;
                    task.Description = $"[cyan]{taskName}[/] - Wave {update.CurrentWave}/{config.MaxWaves}";
                    
                    if (outputLevel >= OutputLevel.Verbose)
                    {
                        task.Description += $" | Gold: {update.CurrentGold} | Lives: {update.RemainingLives}";
                    }
                });

                return await runner.RunSimulationAsync(config, progress);
            });
    }

    private static void DisplayResults(SimulationResult result, OutputLevel outputLevel)
    {
        var status = result.IsVictory ? "[green]✅ VICTORY[/]" : "[red]❌ DEFEAT[/]";
        var finalStats = $"[yellow]Final Gold:[/] {result.FinalMoney} | [yellow]Lives:[/] {result.FinalLives}";

        if (outputLevel == OutputLevel.Minimal)
        {
            AnsiConsole.MarkupLine($"{status} | {finalStats}");
            return;
        }

        var resultsPanel = new Panel($"{status}\n" +
                                   $"{finalStats}\n" +
                                   $"[yellow]Duration:[/] {result.SimulationDuration.TotalMilliseconds:F0}ms")
        {
            Header = new PanelHeader("🎯 Results"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(resultsPanel);

        if (outputLevel >= OutputLevel.Verbose && result.WaveResults.Any())
        {
            AnsiConsole.WriteLine();
            DisplayWaveBreakdown(result.WaveResults);
        }
    }

    private static void DisplayWaveBreakdown(List<WaveResult> waveResults)
    {
        var table = new Table();
        table.AddColumn("Wave");
        table.AddColumn("Enemies");
        table.AddColumn("Gold Earned");
        table.AddColumn("Lives Lost");
        table.AddColumn("Duration");

        foreach (var wave in waveResults)
        {
            table.AddRow(
                wave.WaveNumber.ToString(),
                wave.EnemiesKilled.ToString(),
                wave.MoneyEarned.ToString(),
                wave.LivesLost.ToString(),
                $"{wave.WaveDuration.TotalMilliseconds:F0}ms"
            );
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayComparison(SimulationResult baseline, SimulationResult modified, OutputLevel outputLevel)
    {
        var table = new Table();
        table.AddColumn("Metric");
        table.AddColumn("Baseline");
        table.AddColumn("Modified");
        table.AddColumn("Change");

        AddComparisonRow(table, "Victory", baseline.IsVictory ? "✅" : "❌", modified.IsVictory ? "✅" : "❌");
        AddComparisonRow(table, "Final Gold", baseline.FinalMoney, modified.FinalMoney);
        AddComparisonRow(table, "Lives", baseline.FinalLives, modified.FinalLives);
        AddComparisonRow(table, "Duration", $"{baseline.SimulationDuration.TotalMilliseconds:F0}ms", 
                                         $"{modified.SimulationDuration.TotalMilliseconds:F0}ms");

        AnsiConsole.Write(table);
    }

    private static void AddComparisonRow(Table table, string metric, object baseline, object modified)
    {
        string change = "";
        if (baseline is int baseInt && modified is int modInt)
        {
            var diff = modInt - baseInt;
            change = diff > 0 ? $"[green]+{diff}[/]" : diff < 0 ? $"[red]{diff}[/]" : "[gray]±0[/]";
        }
        else if (baseline is bool baseBool && modified is bool modBool)
        {
            change = baseBool == modBool ? "[gray]Same[/]" : "[yellow]Changed[/]";
        }

        table.AddRow(metric, baseline.ToString() ?? "", modified.ToString() ?? "", change);
    }

    private static async Task ExportToJson(object data, string filePath)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, options);
        await File.WriteAllTextAsync(filePath, json);
        
        AnsiConsole.MarkupLine($"[green]📁 Results exported to: {filePath}[/]");
    }
}

public enum OutputLevel
{
    Minimal,
    Normal,
    Verbose
}
