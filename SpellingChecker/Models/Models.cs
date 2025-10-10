using System;

namespace SpellingChecker.Models
{
    /// <summary>
    /// Represents the result of a spelling correction operation
    /// </summary>
    public class CorrectionResult
    {
        public string OriginalText { get; set; } = string.Empty;
        public string CorrectedText { get; set; } = string.Empty;
        public string[] Changes { get; set; } = Array.Empty<string>();
        public bool HasChanges => OriginalText != CorrectedText;
    }

    /// <summary>
    /// Represents the result of a translation operation
    /// </summary>
    public class TranslationResult
    {
        public string OriginalText { get; set; } = string.Empty;
        public string TranslatedText { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Application configuration settings
    /// </summary>
    public class AppSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = "https://api.openai.com/v1";
        public string SpellingCorrectionHotkey { get; set; } = "Ctrl+Shift+Alt+Y";
        public string TranslationHotkey { get; set; } = "Ctrl+Shift+Alt+T";
        public bool AutoStartWithWindows { get; set; } = true;
        public string Model { get; set; } = "gpt-4o-mini";
    }

    /// <summary>
    /// Represents a single usage record for API calls
    /// </summary>
    public class UsageRecord
    {
        public DateTime Timestamp { get; set; }
        public string OperationType { get; set; } = string.Empty; // "Correction" or "Translation"
        public string Model { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public decimal Cost { get; set; }
    }

    /// <summary>
    /// Aggregated usage statistics
    /// </summary>
    public class UsageStatistics
    {
        public int TotalCorrectionCount { get; set; }
        public int TotalTranslationCount { get; set; }
        public int TotalPromptTokens { get; set; }
        public int TotalCompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public decimal TotalCost { get; set; }
    }
}
