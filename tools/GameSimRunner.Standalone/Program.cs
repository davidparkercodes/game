using System.CommandLine;
using GameSimRunner.Standalone.ValueObjects;
using Spectre.Console;

namespace GameSimRunner.Standalone;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("GameSimRunner - Tower Defense Balance Testing Tool");

        var scenarioOption = new Option<string>(
            "--scenario",
            "Run a predefined scenario (default, balance-testing)"
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
        var runner = new GameSimRunner();
        
        if (outputLevel >= OutputLevel.Normal)
        {
            DisplayScenarioInfo(config, scenario ?? "default");
        }

        var result = await RunWithProgressBar(runner, config, outputLevel);

        DisplayResults(result, outputLevel);
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

    private static async Task<SimulationResult> RunWithProgressBar(GameSimRunner runner, SimulationConfig config, OutputLevel outputLevel)
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
    }
}

public enum OutputLevel
{
    Minimal,
    Normal,
    Verbose
}
