using System;
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
                var prompt = $"맞춤법과 문법을 교정해주세요. 교정된 텍스트만 반환하고 설명은 하지 마세요:\n\n{text}";
                
                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "system", content = "당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하되, 원문의 의미와 어조를 최대한 유지하세요." },
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
