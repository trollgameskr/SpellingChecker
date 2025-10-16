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
        public string? AppliedToneName { get; set; } = null;
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
    /// Represents the result of a common question answering operation
    /// </summary>
    public class CommonQuestionResult
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
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
        public string ApiKey { get; set; } = string.Empty; // Kept for backward compatibility
        public string ApiEndpoint { get; set; } = "https://api.openai.com/v1";
        public string CommonQuestionHotkey { get; set; } = "Alt+D1";
        public string SpellingCorrectionHotkey { get; set; } = "Alt+D2";
        public string TranslationHotkey { get; set; } = "Alt+D3";
        public string VariableNameSuggestionHotkey { get; set; } = "Alt+D4";
        public bool AutoStartWithWindows { get; set; } = true;
        public string Model { get; set; } = "gpt-4o-mini";
        public List<TonePreset> TonePresets { get; set; } = new List<TonePreset>();
        public string SelectedTonePresetId { get; set; } = string.Empty;
        public bool ShowProgressNotifications { get; set; } = false;
        public string Provider { get; set; } = "OpenAI";
        public Dictionary<string, List<string>> CustomModels { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, string> ProviderApiKeys { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Get the API key for the specified provider
        /// </summary>
        public string GetApiKeyForProvider(string provider)
        {
            // Check provider-specific keys first
            if (ProviderApiKeys != null && ProviderApiKeys.ContainsKey(provider) && !string.IsNullOrEmpty(ProviderApiKeys[provider]))
            {
                return ProviderApiKeys[provider];
            }
            
            // Fall back to legacy ApiKey for backward compatibility
            return ApiKey ?? string.Empty;
        }

        /// <summary>
        /// Set the API key for the specified provider
        /// </summary>
        public void SetApiKeyForProvider(string provider, string apiKey)
        {
            if (ProviderApiKeys == null)
            {
                ProviderApiKeys = new Dictionary<string, string>();
            }
            
            ProviderApiKeys[provider] = apiKey;
            
            // Also update legacy ApiKey field for backward compatibility with current provider
            if (provider == Provider)
            {
                ApiKey = apiKey;
            }
        }
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

    /// <summary>
    /// AI Provider configuration helper
    /// </summary>
    public static class AIProviderConfig
    {
        public static readonly string[] Providers = { "OpenAI", "Anthropic", "Gemini" };

        public static Dictionary<string, string[]> ProviderModels = new Dictionary<string, string[]>
        {
            { "OpenAI", new[] { "gpt-4o", "gpt-4o-mini", "o1", "o1-mini" } },
            { "Anthropic", new[] { "claude-sonnet-4-5", "claude-3-5-sonnet-latest", "claude-3-5-haiku-latest" } },
            { "Gemini", new[] { "gemini-2.5-pro-latest", "gemini-2.5-flash-latest", "gemini-2.0-flash-exp" } }
        };

        public static Dictionary<string, string> ProviderEndpoints = new Dictionary<string, string>
        {
            { "OpenAI", "https://api.openai.com/v1" },
            { "Anthropic", "https://api.anthropic.com/v1" },
            { "Gemini", "https://generativelanguage.googleapis.com/v1beta" }
        };

        public static string[] GetModelsForProvider(string provider, Dictionary<string, List<string>>? customModels = null)
        {
            var defaultModels = ProviderModels.ContainsKey(provider) ? ProviderModels[provider] : new[] { "gpt-4o-mini" };
            
            // If no custom models, return defaults
            if (customModels == null || !customModels.ContainsKey(provider) || customModels[provider] == null || customModels[provider].Count == 0)
            {
                return defaultModels;
            }
            
            // Merge custom models with defaults (custom models first, then defaults that aren't duplicates)
            var allModels = new List<string>(customModels[provider] ?? new List<string>());
            foreach (var model in defaultModels)
            {
                if (!allModels.Contains(model))
                {
                    allModels.Add(model);
                }
            }
            
            return allModels.ToArray();
        }

        public static string GetDefaultEndpoint(string provider)
        {
            return ProviderEndpoints.ContainsKey(provider) ? ProviderEndpoints[provider] : "https://api.openai.com/v1";
        }

        public static string GetDefaultModel(string provider)
        {
            if (provider == "OpenAI")
                return "gpt-4o-mini";
            else if (provider == "Anthropic")
                return "claude-sonnet-4-5";
            else if (provider == "Gemini")
                return "gemini-2.5-flash-latest";
            return "gpt-4o-mini";
        }
    }
}
