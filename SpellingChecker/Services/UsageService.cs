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
            catch
            {
                // Silently fail - don't interrupt user operations
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
            catch
            {
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

        public void ClearHistory()
        {
            try
            {
                if (File.Exists(UsageFilePath))
                {
                    File.Delete(UsageFilePath);
                }
            }
            catch
            {
                // Silently fail
            }
        }
    }
}
