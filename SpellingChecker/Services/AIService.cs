using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpellingChecker.Models;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for AI-powered spelling correction and translation using OpenAI API
    /// </summary>
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsService _settingsService;
        private readonly UsageService _usageService;
        private AppSettings _settings;

        public AIService(SettingsService settingsService)
        {
            _httpClient = new HttpClient();
            _settingsService = settingsService;
            _usageService = new UsageService();
            _settings = _settingsService.LoadSettings();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<CorrectionResult> CorrectSpellingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new CorrectionResult { OriginalText = text, CorrectedText = text };
            }

            try
            {
                // Get the selected tone preset
                var tonePreset = TonePresetService.GetSelectedTonePreset(_settings);
                
                string prompt;
                string systemMessage = "당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하되, 원문의 의미와 어조를 최대한 유지하세요.";
                
                if (tonePreset != null && tonePreset.Id != "default-none")
                {
                    // Apply tone to the corrected text
                    prompt = $"맞춤법과 문법을 교정하고, 다음 톤으로 변환해주세요.\n\n톤: {tonePreset.Name}\n설명: {tonePreset.Description}\n\n교정 및 톤 변환된 텍스트만 반환하고 설명은 하지 마세요:\n\n{text}";
                    systemMessage = $"당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하고, 지정된 톤({tonePreset.Description})으로 자연스럽게 변환하세요.";
                }
                else
                {
                    // No tone applied, just correction
                    prompt = $"맞춤법과 문법을 교정해주세요. 교정된 텍스트만 반환하고 설명은 하지 마세요:\n\n{text}";
                }
                
                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "system", content = systemMessage },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.3,
                    max_tokens = 2000
                };

                var response = await SendRequestAsync(requestBody);
                var correctedText = ExtractContentFromResponse(response);
                RecordUsageFromResponse(response, "Correction");

                return new CorrectionResult
                {
                    OriginalText = text,
                    CorrectedText = correctedText.Trim(),
                    Changes = Array.Empty<string>() // Could be enhanced to show specific changes
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Spelling correction failed: {ex.Message}", ex);
            }
        }

        public async Task<TranslationResult> TranslateAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new TranslationResult { OriginalText = text, TranslatedText = text };
            }

            try
            {
                var detectedLanguage = DetectLanguage(text);
                var targetLanguage = detectedLanguage == "Korean" ? "English" : "Korean";
                
                var prompt = detectedLanguage == "Korean" 
                    ? $"다음 한국어 텍스트를 영어로 번역해주세요. 번역 결과만 반환하고 설명은 하지 마세요:\n\n{text}"
                    : $"Translate the following English text to Korean. Return only the translation without explanations:\n\n{text}";

                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "system", content = "당신은 전문 번역가입니다. 정확하고 자연스러운 번역을 제공하세요." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.3,
                    max_tokens = 2000
                };

                var response = await SendRequestAsync(requestBody);
                var translatedText = ExtractContentFromResponse(response);
                RecordUsageFromResponse(response, "Translation");

                return new TranslationResult
                {
                    OriginalText = text,
                    TranslatedText = translatedText.Trim(),
                    SourceLanguage = detectedLanguage,
                    TargetLanguage = targetLanguage
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Translation failed: {ex.Message}", ex);
            }
        }

        public async Task<VariableNameSuggestionResult> SuggestVariableNamesAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new VariableNameSuggestionResult { OriginalText = text, SuggestedNames = Array.Empty<string>() };
            }

            try
            {
                var prompt = $"다음 한글 텍스트를 C# 변수명 규칙에 맞게 3개의 변수명을 추천해주세요. 각 변수명은 camelCase 형식이어야 하며, 의미가 명확해야 합니다.\n\n텍스트: {text}\n\n각 변수명을 새 줄로 구분하여 반환하고, 설명이나 번호는 붙이지 마세요. 변수명만 반환하세요.";

                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "system", content = "당신은 C# 프로그래밍 전문가입니다. 한글 텍스트를 의미있는 영어 변수명으로 변환하는 것을 도와줍니다. Microsoft C# 명명 규칙을 따르며, camelCase를 사용합니다." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.5,
                    max_tokens = 200
                };

                var response = await SendRequestAsync(requestBody);
                var suggestionsText = ExtractContentFromResponse(response);
                RecordUsageFromResponse(response, "VariableNameSuggestion");

                // Parse the suggestions - split by newlines and take first 3 non-empty lines
                var suggestions = suggestionsText
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Take(3)
                    .ToArray();

                return new VariableNameSuggestionResult
                {
                    OriginalText = text,
                    SuggestedNames = suggestions
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Variable name suggestion failed: {ex.Message}", ex);
            }
        }

        private async Task<string> SendRequestAsync(object requestBody)
        {
            _settings = _settingsService.LoadSettings(); // Reload settings for API key updates
            
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                throw new InvalidOperationException("API Key is not configured. Please set your OpenAI API key in settings.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.ApiEndpoint}/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
            }

            return responseContent;
        }

        private string ExtractContentFromResponse(string response)
        {
            try
            {
                var json = JObject.Parse(response);
                var content = json["choices"]?[0]?["message"]?["content"]?.ToString();
                return content ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string DetectLanguage(string text)
        {
            // Simple language detection based on character ranges
            foreach (var c in text)
            {
                if (c >= 0xAC00 && c <= 0xD7A3) // Hangul syllables
                {
                    return "Korean";
                }
            }
            return "English";
        }

        private void RecordUsageFromResponse(string response, string operationType)
        {
            try
            {
                var json = JObject.Parse(response);
                var usage = json["usage"];
                if (usage != null)
                {
                    var promptTokens = usage["prompt_tokens"]?.Value<int>() ?? 0;
                    var completionTokens = usage["completion_tokens"]?.Value<int>() ?? 0;
                    _usageService.RecordUsage(operationType, _settings.Model, promptTokens, completionTokens);
                }
            }
            catch
            {
                // Silently fail - don't interrupt user operations
            }
        }
    }
}
