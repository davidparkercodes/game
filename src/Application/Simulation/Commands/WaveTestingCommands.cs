using System;
using System.Collections.Generic;
using System.IO;
using Game.Application.Simulation.ValueObjects;
using Game.Application.Simulation.Tools;

namespace Game.Application.Simulation.Commands;

public static class WaveTestingCommands
{
    public static void RunQuickBalanceTest()
    {
        Console.WriteLine("🧪 Quick Wave Balance Test");
        Console.WriteLine("=========================");
        
        var runner = new GameSimRunner();
        var config = SimulationConfig.ForBalanceTesting();
        
        Console.WriteLine($"Testing wave set: {config.WaveSetDifficulty}");
        Console.WriteLine($"Max waves: {config.MaxWaves}");
        Console.WriteLine();
        
        var result = runner.RunSimulation(config, new Progress<SimulationProgress>(progress =>
        {
            var progressBar = WaveTestingTools.GenerateWaveProgressBar(
                progress.CurrentWave, config.MaxWaves, 
                progress.CurrentWave / (float)config.MaxWaves, 
                progress.CurrentGold, progress.RemainingLives);
            Console.Write($"\r{progressBar}");
        }));
        
        Console.WriteLine();
        Console.WriteLine();
        
        var metrics = runner.GetSimulationMetrics("Quick Balance Test", result.SimulationDuration, result.Success);
        Console.WriteLine(WaveTestingTools.GenerateSimulationSummary(metrics));
        
        if (!result.Success)
        {
            Console.WriteLine($"❌ Test failed: {result.FailureReason}");
        }
    }
    
    public static void RunComprehensiveBalanceTest()
    {
        Console.WriteLine("🔬 Comprehensive Wave Balance Test");
        Console.WriteLine("==================================");
        
        var runner = new GameSimRunner();
        var testResults = new List<SimulationMetrics>();
        
        var testScenarios = new[]
        {
            ("Default", SimulationConfig.Default()),
            ("Balance Testing", SimulationConfig.ForBalanceTesting()),
            ("Easy Mode", new SimulationConfig(enemyHealthMultiplier: 0.8f, maxWaves: 5)),
            ("Hard Mode", new SimulationConfig(enemyHealthMultiplier: 1.2f, maxWaves: 5)),
            ("Double Enemies", new SimulationConfig(enemyCountMultiplier: 2.0f, maxWaves: 3))
        };
        
        foreach (var (name, config) in testScenarios)
        {
            Console.WriteLine($"Running scenario: {name}...");
            
            var result = runner.RunSimulation(config);
            var metrics = runner.GetSimulationMetrics(name, result.SimulationDuration, result.Success);
            testResults.Add(metrics);
            
            Console.WriteLine($"  Result: {(result.Success ? "✅ PASS" : "❌ FAIL")}");
        }
        
        Console.WriteLine();
        Console.WriteLine(WaveTestingTools.GenerateBalanceTestingReport(testResults));
    }
    
    public static void ValidateWaveConfigurations()
    {
        Console.WriteLine("🔍 Wave Configuration Validation");
        Console.WriteLine("================================");
        
        var configFiles = new[]
        {
            "config/levels/wave-configs.json",
            "config/levels/wave-configs-balance.json",
            "config/levels/default_waves.json"
        };
        
        foreach (var configFile in configFiles)
        {
            Console.WriteLine($"\nValidating: {configFile}");
            var issues = WaveTestingTools.ValidateWaveConfiguration(configFile);
            
            foreach (var issue in issues)
            {
                Console.WriteLine($"  {issue}");
            }
        }
    }
    
    public static void RunWaveDifficultyAnalysis()
    {
        Console.WriteLine("📈 Wave Difficulty Analysis");
        Console.WriteLine("===========================");
        
        var runner = new GameSimRunner();
        var config = SimulationConfig.Default();
        
        Console.WriteLine("Running difficulty analysis simulation...");
        var result = runner.RunSimulation(config);
        var metrics = runner.GetSimulationMetrics("Difficulty Analysis", result.SimulationDuration, result.Success);
        
        Console.WriteLine();
        Console.WriteLine(WaveTestingTools.GenerateWaveProgressionChart(metrics.WaveMetrics));
        Console.WriteLine();
        Console.WriteLine(WaveTestingTools.GenerateSimulationSummary(metrics));
    }
    
    public static void ExportDetailedAnalysis(string outputDirectory = "simulation_reports")
    {
        Console.WriteLine("📊 Exporting Detailed Analysis");
        Console.WriteLine("==============================");
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
            Console.WriteLine($"Created output directory: {outputDirectory}");
        }
        
        var runner = new GameSimRunner();
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        
        var scenarios = new[]
        {
            ("default", SimulationConfig.Default()),
            ("balance_testing", SimulationConfig.ForBalanceTesting()),
            ("difficulty_test", SimulationConfig.WithDifficultyModifier(1.5f))
        };
        
        foreach (var (name, config) in scenarios)
        {
            Console.WriteLine($"Analyzing scenario: {name}...");
            
            var result = runner.RunSimulation(config);
            var metrics = runner.GetSimulationMetrics($"{name}_scenario", result.SimulationDuration, result.Success);
            
            // Export detailed report
            var reportPath = Path.Combine(outputDirectory, $"{name}_{timestamp}.md");
            WaveTestingTools.ExportDetailedReport(metrics, reportPath);
            Console.WriteLine($"  Exported: {reportPath}");
            
            // Export metrics JSON
            var metricsPath = Path.Combine(outputDirectory, $"{name}_{timestamp}_metrics.json");
            runner.ExportMetrics(metricsPath, metrics);
            Console.WriteLine($"  Exported: {metricsPath}");
        }
        
        Console.WriteLine($"\nAll reports exported to: {Path.GetFullPath(outputDirectory)}");
    }
    
    public static void ShowWaveTestingHelp()
    {
        Console.WriteLine("🔧 Wave Testing Tools - Available Commands");
        Console.WriteLine("==========================================");
        Console.WriteLine();
        Console.WriteLine("quick-balance     - Run quick balance test with progress indicators");
        Console.WriteLine("comprehensive     - Run comprehensive balance test across multiple scenarios");
        Console.WriteLine("validate-configs  - Validate all wave configuration files");
        Console.WriteLine("difficulty        - Analyze wave difficulty progression");
        Console.WriteLine("export-analysis   - Export detailed analysis reports and metrics");
        Console.WriteLine("help              - Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  WaveTestingCommands.RunQuickBalanceTest();");
        Console.WriteLine("  WaveTestingCommands.ExportDetailedAnalysis(\"my_reports\");");
    }
}
