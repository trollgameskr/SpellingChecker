using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SpellingChecker.Models;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for tracking and managing API usage statistics
    /// </summary>
    public class UsageService
    {
        private static readonly string UsageFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SpellingChecker",
            "usage_history.json"
        );

        // Pricing per 1M tokens for different models
        private static readonly Dictionary<string, (decimal input, decimal output)> ModelPricing = new()
        {
            { "gpt-4o-mini", (0.15m, 0.60m) },
            { "gpt-4o", (5.00m, 15.00m) },
            { "gpt-3.5-turbo", (0.50m, 1.50m) }
        };

        public void RecordUsage(string operationType, string model, int promptTokens, int completionTokens)
        {
            var totalTokens = promptTokens + completionTokens;
            var cost = CalculateCost(model, promptTokens, completionTokens);

            var record = new UsageRecord
            {
                Timestamp = DateTime.Now,
                OperationType = operationType,
                Model = model,
                PromptTokens = promptTokens,
                CompletionTokens = completionTokens,
                TotalTokens = totalTokens,
                Cost = cost
            };

            SaveUsageRecord(record);
        }

        private decimal CalculateCost(string model, int promptTokens, int completionTokens)
        {
            if (ModelPricing.TryGetValue(model, out var pricing))
            {
                var inputCost = (promptTokens / 1_000_000m) * pricing.input;
                var outputCost = (completionTokens / 1_000_000m) * pricing.output;
                return inputCost + outputCost;
            }
            return 0m; // Unknown model
        }

        private void SaveUsageRecord(UsageRecord record)
        {
            try
            {
                var records = LoadAllRecords();
                records.Add(record);

                var directory = Path.GetDirectoryName(UsageFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(records, Formatting.Indented);
                File.WriteAllText(UsageFilePath, json);
            }
            catch (Exception ex)
            {
                // Silently fail - don't interrupt user operations
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to save usage record: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
            }
        }

        public List<UsageRecord> LoadAllRecords()
        {
            try
            {
                if (!File.Exists(UsageFilePath))
                {
                    return new List<UsageRecord>();
                }

                var json = File.ReadAllText(UsageFilePath);
                return JsonConvert.DeserializeObject<List<UsageRecord>>(json) ?? new List<UsageRecord>();
            }
            catch (Exception ex)
            {
                // Return empty list if file is corrupted or unreadable
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to load usage records: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                return new List<UsageRecord>();
            }
        }

        public UsageStatistics GetStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var records = LoadAllRecords();

            if (startDate.HasValue)
            {
                records = records.Where(r => r.Timestamp >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                records = records.Where(r => r.Timestamp <= endDate.Value).ToList();
            }

            return new UsageStatistics
            {
                TotalCorrectionCount = records.Count(r => r.OperationType == "Correction"),
                TotalTranslationCount = records.Count(r => r.OperationType == "Translation"),
                TotalPromptTokens = records.Sum(r => r.PromptTokens),
                TotalCompletionTokens = records.Sum(r => r.CompletionTokens),
                TotalTokens = records.Sum(r => r.TotalTokens),
                TotalCost = records.Sum(r => r.Cost)
            };
        }

        public List<DailyUsageStatistics> GetDailyStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var records = LoadAllRecords();

            if (startDate.HasValue)
            {
                records = records.Where(r => r.Timestamp >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                records = records.Where(r => r.Timestamp <= endDate.Value).ToList();
            }

            // Group by date and aggregate
            var dailyStats = records
                .GroupBy(r => r.Timestamp.Date)
                .Select(g => new DailyUsageStatistics
                {
                    Date = g.Key,
                    OperationCount = g.Count(),
                    CorrectionCount = g.Count(r => r.OperationType == "Correction"),
                    TranslationCount = g.Count(r => r.OperationType == "Translation"),
                    TotalTokens = g.Sum(r => r.TotalTokens),
                    Cost = g.Sum(r => r.Cost)
                })
                .OrderByDescending(d => d.Date)
                .ToList();

            return dailyStats;
        }

        public List<MonthlyUsageStatistics> GetMonthlyStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var records = LoadAllRecords();

            if (startDate.HasValue)
            {
                records = records.Where(r => r.Timestamp >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                records = records.Where(r => r.Timestamp <= endDate.Value).ToList();
            }

            // Group by year and month and aggregate
            var monthlyStats = records
                .GroupBy(r => new { r.Timestamp.Year, r.Timestamp.Month })
                .Select(g => new MonthlyUsageStatistics
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    OperationCount = g.Count(),
                    CorrectionCount = g.Count(r => r.OperationType == "Correction"),
                    TranslationCount = g.Count(r => r.OperationType == "Translation"),
                    TotalTokens = g.Sum(r => r.TotalTokens),
                    Cost = g.Sum(r => r.Cost)
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList();

            return monthlyStats;
        }

        public void ClearHistory()
        {
            try
            {
                if (File.Exists(UsageFilePath))
                {
                    File.Delete(UsageFilePath);
                }
            }
            catch (Exception ex)
            {
                // Silently fail - file may be locked or inaccessible
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to clear history: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
            }
        }
    }
}
