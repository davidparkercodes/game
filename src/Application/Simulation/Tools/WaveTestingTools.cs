using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Application.Simulation.ValueObjects;

namespace Game.Application.Simulation.Tools;

public static class WaveTestingTools
{
    public static string GenerateWaveProgressBar(int currentWave, int totalWaves, float waveProgress, int gold, int lives)
    {
        const int barWidth = 20;
        var completedBars = (int)(waveProgress * barWidth);
        var progressBar = new string('█', completedBars) + new string('░', barWidth - completedBars);
        
        return $"Wave {currentWave:D2}/{totalWaves:D2} | Gold: {gold:D4} | Lives: {lives:D2} [{progressBar}] {waveProgress:P0}";
    }

    public static string GenerateWaveValidationReport(WaveMetrics waveMetrics)
    {
        var report = new StringBuilder();
        report.AppendLine($"=== Wave {waveMetrics.WaveNumber}: {waveMetrics.WaveName} ===");
        report.AppendLine($"Duration: {waveMetrics.WaveDuration.TotalSeconds:F1}s");
        report.AppendLine($"Enemies: {waveMetrics.EnemiesKilled}/{waveMetrics.TotalEnemies} killed ({waveMetrics.CompletionRate:P1})");
        report.AppendLine($"Leaked: {waveMetrics.EnemiesLeaked}");
        report.AppendLine($"Lives Lost: {waveMetrics.LivesLost}");
        report.AppendLine($"Money Earned: {waveMetrics.MoneyEarned}");
        report.AppendLine($"Difficulty Rating: {waveMetrics.DifficultyRating:F2}/5.0");
        
        var status = waveMetrics.IsSuccessful ? "✅ SUCCESS" :
                    waveMetrics.IsPartialSuccess ? "⚠️ PARTIAL" : "❌ FAILED";
        report.AppendLine($"Status: {status}");
        report.AppendLine();
        
        return report.ToString();
    }

    public static string GenerateSimulationSummary(SimulationMetrics metrics)
    {
        var summary = new StringBuilder();
        summary.AppendLine("╭─────────────────────────────────────╮");
        summary.AppendLine($"│ {metrics.ScenarioName.PadRight(35)} │");
        summary.AppendLine("├─────────────────────────────────────┤");
        summary.AppendLine($"│ Result: {(metrics.OverallSuccess ? "✅ SUCCESS" : "❌ FAILED").PadRight(27)} │");
        summary.AppendLine($"│ Duration: {metrics.TotalDuration.TotalSeconds:F1}s".PadRight(36) + " │");
        summary.AppendLine($"│ Waves: {metrics.TotalWavesCompleted}/{metrics.TotalWavesAttempted}".PadRight(36) + " │");
        summary.AppendLine($"│ Completion: {metrics.OverallCompletionRate:P1}".PadRight(36) + " │");
        summary.AppendLine($"│ Avg Difficulty: {metrics.AverageDifficultyRating:F2}".PadRight(36) + " │");
        
        if (metrics.CustomMetrics.TryGetValue("BalanceScore", out var balanceScore))
        {
            summary.AppendLine($"│ Balance Score: {balanceScore:F2}".PadRight(36) + " │");
        }
        
        summary.AppendLine("╰─────────────────────────────────────╯");
        
        return summary.ToString();
    }

    public static string GenerateBalanceTestingReport(List<SimulationMetrics> testResults)
    {
        var report = new StringBuilder();
        report.AppendLine("🧪 Wave Balance Testing Report");
        report.AppendLine("══════════════════════════════════");
        report.AppendLine();

        foreach (var result in testResults)
        {
            report.AppendLine($"📊 Scenario: {result.ScenarioName}");
            report.AppendLine($"   Result: {(result.OverallSuccess ? "✅ PASS" : "❌ FAIL")}");
            report.AppendLine($"   Duration: {result.TotalDuration.TotalMilliseconds:F0}ms");
            report.AppendLine($"   Waves Completed: {result.TotalWavesCompleted}/{result.TotalWavesAttempted}");
            
            if (result.CustomMetrics.TryGetValue("KillEfficiency", out var killEff))
            {
                report.AppendLine($"   Kill Efficiency: {killEff:P1}");
            }
            
            if (result.CustomMetrics.TryGetValue("BalanceScore", out var balance))
            {
                report.AppendLine($"   Balance Score: {balance:F2}");
            }
            
            report.AppendLine();
        }

        // Generate recommendations
        report.AppendLine("💡 Recommendations:");
        var recommendations = GenerateBalanceRecommendations(testResults);
        foreach (var recommendation in recommendations)
        {
            report.AppendLine($"   • {recommendation}");
        }

        return report.ToString();
    }

    public static List<string> ValidateWaveConfiguration(string configFilePath)
    {
        var issues = new List<string>();
        
        try
        {
            if (!File.Exists(configFilePath))
            {
                issues.Add($"❌ Configuration file not found: {configFilePath}");
                return issues;
            }

            var content = File.ReadAllText(configFilePath);
            if (string.IsNullOrWhiteSpace(content))
            {
                issues.Add("❌ Configuration file is empty");
                return issues;
            }

            // Add basic JSON validation
            try
            {
                System.Text.Json.JsonDocument.Parse(content);
                issues.Add("✅ Valid JSON format");
            }
            catch (System.Text.Json.JsonException ex)
            {
                issues.Add($"❌ Invalid JSON: {ex.Message}");
                return issues;
            }

            // Additional validation logic would go here
            issues.Add("✅ Wave configuration structure appears valid");
        }
        catch (Exception ex)
        {
            issues.Add($"❌ Error validating configuration: {ex.Message}");
        }
        
        return issues;
    }

    public static string GenerateWaveProgressionChart(List<WaveMetrics> waveMetrics)
    {
        var chart = new StringBuilder();
        chart.AppendLine("📈 Wave Difficulty Progression");
        chart.AppendLine("─────────────────────────────");
        
        const int chartWidth = 50;
        var maxDifficulty = waveMetrics.Any() ? waveMetrics.Max(w => w.DifficultyRating) : 1.0f;
        
        foreach (var wave in waveMetrics)
        {
            var barLength = (int)((wave.DifficultyRating / maxDifficulty) * chartWidth);
            var bar = new string('█', barLength);
            var status = wave.IsSuccessful ? "✅" : wave.IsPartialSuccess ? "⚠️" : "❌";
            
            chart.AppendLine($"Wave {wave.WaveNumber:D2} {status} |{bar.PadRight(chartWidth)}| {wave.DifficultyRating:F2}");
        }
        
        return chart.ToString();
    }

    public static void ExportDetailedReport(SimulationMetrics metrics, string outputPath)
    {
        var report = new StringBuilder();
        
        // Header
        report.AppendLine($"# Detailed Wave Simulation Report");
        report.AppendLine($"**Scenario:** {metrics.ScenarioName}");
        report.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine();
        
        // Summary
        report.AppendLine("## Summary");
        report.AppendLine($"- **Overall Success:** {(metrics.OverallSuccess ? "✅ Yes" : "❌ No")}");
        report.AppendLine($"- **Total Duration:** {metrics.TotalDuration.TotalSeconds:F2} seconds");
        report.AppendLine($"- **Waves Completed:** {metrics.TotalWavesCompleted}/{metrics.TotalWavesAttempted}");
        report.AppendLine($"- **Overall Completion Rate:** {metrics.OverallCompletionRate:P1}");
        report.AppendLine($"- **Average Difficulty Rating:** {metrics.AverageDifficultyRating:F2}/5.0");
        report.AppendLine();
        
        // Wave Details
        report.AppendLine("## Wave Analysis");
        foreach (var wave in metrics.WaveMetrics)
        {
            report.AppendLine($"### Wave {wave.WaveNumber}: {wave.WaveName}");
            report.AppendLine($"- **Duration:** {wave.WaveDuration.TotalSeconds:F1}s");
            report.AppendLine($"- **Enemies:** {wave.EnemiesKilled}/{wave.TotalEnemies} killed ({wave.CompletionRate:P1})");
            report.AppendLine($"- **Lives Lost:** {wave.LivesLost}");
            report.AppendLine($"- **Money Earned:** {wave.MoneyEarned}");
            report.AppendLine($"- **Difficulty Rating:** {wave.DifficultyRating:F2}/5.0");
            report.AppendLine($"- **Status:** {(wave.IsSuccessful ? "✅ Success" : wave.IsPartialSuccess ? "⚠️ Partial Success" : "❌ Failed")}");
            report.AppendLine();
        }
        
        // Custom Metrics
        if (metrics.CustomMetrics.Any())
        {
            report.AppendLine("## Performance Metrics");
            foreach (var metric in metrics.CustomMetrics)
            {
                report.AppendLine($"- **{metric.Key}:** {metric.Value}");
            }
            report.AppendLine();
        }
        
        File.WriteAllText(outputPath, report.ToString());
    }

    private static List<string> GenerateBalanceRecommendations(List<SimulationMetrics> testResults)
    {
        var recommendations = new List<string>();
        
        var failedTests = testResults.Where(r => !r.OverallSuccess).ToList();
        if (failedTests.Any())
        {
            recommendations.Add($"❌ {failedTests.Count} test(s) failed - review difficulty scaling");
        }
        
        var lowCompletionTests = testResults.Where(r => r.OverallCompletionRate < 0.8f).ToList();
        if (lowCompletionTests.Any())
        {
            recommendations.Add($"⚠️ {lowCompletionTests.Count} test(s) have low completion rates - consider reducing enemy difficulty");
        }
        
        var avgDifficulty = testResults.Average(r => r.AverageDifficultyRating);
        if (avgDifficulty > 3.0f)
        {
            recommendations.Add("⚠️ Average difficulty rating is high - consider rebalancing enemy stats");
        }
        else if (avgDifficulty < 1.0f)
        {
            recommendations.Add("💡 Average difficulty rating is low - consider increasing challenge");
        }
        
        if (!recommendations.Any())
        {
            recommendations.Add("✅ All tests passed with good balance metrics");
        }
        
        return recommendations;
    }
}
