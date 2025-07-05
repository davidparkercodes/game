using System.CommandLine;
using Game.Application.Simulation;
using Game.Application.Simulation.ValueObjects;
using Spectre.Console;

namespace GameSimRunner;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("GameSimRunner - Tower Defense Balance Testing Tool");

        var scenarioOption = new Option<string>(
            "--scenario",
            "Run a predefined scenario (e.g., default, balance-testing)"
        );

        var verboseOption = new Option<bool>(
            "--verbose",
            "Enable verbose output with detailed simulation data"
        );

        var minimalOption = new Option<bool>(
            "--minimal",
            "Enable minimal output (overrides verbose)"
        );

        rootCommand.AddOption(scenarioOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(minimalOption);

        rootCommand.SetHandler(async (scenario, verbose, minimal) =>
        {
            try
            {
                var outputLevel = minimal ? OutputLevel.Minimal : 
                                 verbose ? OutputLevel.Verbose : 
                                 OutputLevel.Normal;

                await RunSimulation(scenario, outputLevel);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }, scenarioOption, verboseOption, minimalOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task RunSimulation(string? scenario, OutputLevel outputLevel)
    {
        AnsiConsole.MarkupLine("[bold cyan]🎯 GameSimRunner - Tower Defense Balance Testing[/]");
        AnsiConsole.WriteLine();

        var config = GetConfig(scenario);
        
        // Debug: Show working directory and config loading
        if (outputLevel >= OutputLevel.Verbose)
        {
            AnsiConsole.MarkupLine($"[dim]Working Directory: {Environment.CurrentDirectory}[/]");
            AnsiConsole.MarkupLine($"[dim]Looking for config files...[/]");
        }
        
        // Validate configuration files exist
        if (!ValidateConfigurationFiles(outputLevel))
        {
            AnsiConsole.MarkupLine("[red]❌ Configuration validation failed. Cannot proceed with simulation.[/]");
            return;
        }
        
        Game.Application.Simulation.GameSimRunner runner;
        try
        {
            runner = new Game.Application.Simulation.GameSimRunner();
            if (outputLevel >= OutputLevel.Verbose)
            {
                AnsiConsole.MarkupLine($"[green]✅ GameSimRunner initialized successfully[/]");
            }
        }
        catch (FileNotFoundException ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Configuration file not found: {ex.FileName}[/]");
            AnsiConsole.MarkupLine("[yellow]💡 Ensure that data/simulation/ directory contains the required JSON config files.[/]");
            return;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Failed to initialize GameSimRunner: {ex.Message}[/]");
            if (outputLevel >= OutputLevel.Verbose)
            {
                AnsiConsole.MarkupLine($"[red]Stack trace: {ex.StackTrace}[/]");
            }
            return;
        }
        
        if (outputLevel >= OutputLevel.Normal)
        {
            DisplayScenarioInfo(config, scenario ?? "default");
        }

        try
        {
            var result = await RunWithProgressBar(runner, config, outputLevel);
            DisplayResults(result, outputLevel);
            
            // Debug: Show failure reason if available
            if (!result.Success && outputLevel >= OutputLevel.Verbose)
            {
                AnsiConsole.MarkupLine($"[red]Failure Details: {result.ToString()}[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Simulation failed: {ex.Message}[/]");
            if (outputLevel >= OutputLevel.Verbose)
            {
                AnsiConsole.MarkupLine($"[red]Stack trace: {ex.StackTrace}[/]");
            }
        }
    }

    private static SimulationConfig GetConfig(string? scenario)
    {
        return scenario?.ToLower() switch
        {
            "balance-testing" => SimulationConfig.ForBalanceTesting(),
            _ => SimulationConfig.Default()
        };
    }

    private static void DisplayScenarioInfo(SimulationConfig config, string scenarioName)
    {
        var panel = new Panel($"[yellow]Scenario:[/] {scenarioName}\n" +
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

    private static async Task<SimulationResult> RunWithProgressBar(Game.Application.Simulation.GameSimRunner runner, SimulationConfig config, OutputLevel outputLevel)
    {
        return await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[cyan]Simulation[/]", maxValue: config.MaxWaves);
                
                var progress = new Progress<SimulationProgress>(update =>
                {
                    task.Value = update.CurrentWave;
                    task.Description = $"[cyan]Simulation[/] - Wave {update.CurrentWave}/{config.MaxWaves}";
                    
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
            AnsiConsole.MarkupLine($"{status} | {finalStats} | Score: {result.FinalScore} | Enemies: {result.TotalEnemiesKilled}");
            return;
        }

        // Display wave-by-wave breakdown in verbose mode
        if (outputLevel >= OutputLevel.Verbose && result.WaveResults.Count > 0)
        {
            DisplayWaveBreakdown(result.WaveResults);
            AnsiConsole.WriteLine();
        }

        var detailedStats = outputLevel >= OutputLevel.Verbose 
            ? $"{status}\n" +
              $"{finalStats}\n" +
              $"[yellow]Final Score:[/] {result.FinalScore}\n" +
              $"[yellow]Waves Completed:[/] {result.WavesCompleted}\n" +
              $"[yellow]Total Enemies Killed:[/] {result.TotalEnemiesKilled}\n" +
              $"[yellow]Buildings Placed:[/] {result.TotalBuildingsPlaced}\n" +
              $"[yellow]Duration:[/] {result.SimulationDuration.TotalMilliseconds:F0}ms"
            : $"{status}\n" +
              $"{finalStats}\n" +
              $"[yellow]Duration:[/] {result.SimulationDuration.TotalMilliseconds:F0}ms";

        var resultsPanel = new Panel(detailedStats)
        {
            Header = new PanelHeader("🎯 Results"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(resultsPanel);
        
        // Show failure reason if applicable
        if (!result.Success && !string.IsNullOrEmpty(result.FailureReason))
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[red]💥 Failure Reason: {result.FailureReason}[/]");
        }
    }
    
    private static void DisplayWaveBreakdown(List<Game.Application.Simulation.ValueObjects.WaveResult> waveResults)
    {
        AnsiConsole.MarkupLine("[bold cyan]📊 Wave-by-Wave Breakdown[/]");
        
        var table = new Table()
            .AddColumn("[yellow]Wave[/]")
            .AddColumn("[green]Status[/]")
            .AddColumn("[cyan]Enemies[/]")
            .AddColumn("[red]Lives Lost[/]")
            .AddColumn("[yellow]Money[/]")
            .AddColumn("[blue]Score[/]")
            .AddColumn("[dim]Duration[/]");
            
        table.Border = TableBorder.Rounded;
        
        foreach (var wave in waveResults)
        {
            var status = wave.Completed ? "[green]✓[/]" : "[red]✗[/]";
            var duration = $"{wave.WaveDuration.TotalMilliseconds:F0}ms";
            
            table.AddRow(
                wave.WaveNumber.ToString(),
                status,
                wave.EnemiesKilled.ToString(),
                wave.LivesLost.ToString(),
                $"+{wave.MoneyEarned}",
                $"+{wave.ScoreEarned}",
                duration
            );
        }
        
        AnsiConsole.Write(table);
    }
    
    private static bool ValidateConfigurationFiles(OutputLevel outputLevel)
    {
        var configFiles = new[]
        {
            "data/simulation/building-stats.json",
            "data/simulation/enemy-stats.json"
        };
        
        var allFilesExist = true;
        
        foreach (var configFile in configFiles)
        {
            if (!File.Exists(configFile))
            {
                AnsiConsole.MarkupLine($"[red]❌ Missing config file: {configFile}[/]");
                allFilesExist = false;
            }
            else if (outputLevel >= OutputLevel.Verbose)
            {
                AnsiConsole.MarkupLine($"[green]✓ Found config file: {configFile}[/]");
            }
        }
        
        if (!allFilesExist)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]💡 Expected configuration files:[/]");
            foreach (var file in configFiles)
            {
                AnsiConsole.MarkupLine($"  - {file}");
            }
        }
        
        return allFilesExist;
    }
}

public enum OutputLevel
{
    Minimal,
    Normal,
    Verbose
}
