using System;
using System.Collections.Generic;

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
    /// Represents the result of a variable name suggestion operation
    /// </summary>
    public class VariableNameSuggestionResult
    {
        public string OriginalText { get; set; } = string.Empty;
        public string[] SuggestedNames { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// AI Provider types
    /// </summary>
    public enum AIProvider
    {
        OpenAI,
        AzureOpenAI,
        Custom
    }

    /// <summary>
    /// Represents a tone preset for spelling correction
    /// </summary>
    public class TonePreset
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
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
        public string VariableNameSuggestionHotkey { get; set; } = "Ctrl+Shift+Alt+V";
        public bool AutoStartWithWindows { get; set; } = true;
        public string Model { get; set; } = "gpt-4o-mini";
        public AIProvider Provider { get; set; } = AIProvider.OpenAI;
        public List<TonePreset> TonePresets { get; set; } = new List<TonePreset>();
        public string SelectedTonePresetId { get; set; } = string.Empty;
        public bool ShowProgressNotifications { get; set; } = false;
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
