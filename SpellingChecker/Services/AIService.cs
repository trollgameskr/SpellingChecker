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
            // Use configurable timeout from settings, default to 60 seconds
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.RequestTimeoutSeconds > 0 ? _settings.RequestTimeoutSeconds : 60);
        }

        public async Task<CorrectionResult> CorrectSpellingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new CorrectionResult { OriginalText = text, CorrectedText = text };
            }

            try
            {
                // Reload settings to get the latest tone preset selection
                _settings = _settingsService.LoadSettings();
                
                // Get the selected tone preset
                var tonePreset = TonePresetService.GetSelectedTonePreset(_settings);
                
                string prompt;
                string systemMessage = "당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하되, 원문의 의미와 어조를 최대한 유지하세요.";
                
                if (tonePreset != null && tonePreset.Id != "default-none")
                {
                    // Apply tone to the corrected text
                    prompt = $"맞춤법과 문법을 교정하고, 원문의 톤은 완전히 무시한 채 오직 내용만 유지하여 다음 톤으로 변환해주세요.\n\n톤: {tonePreset.Name}\n설명: {tonePreset.Description}\n\n교정 및 톤 변환된 텍스트만 반환하고 설명은 하지 마세요:\n\n{text}";
                    systemMessage = $"당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하고, 원문의 톤이나 말투는 완전히 무시한 채 오직 의미만 유지하면서 지정된 톤({tonePreset.Description})으로 완전히 새롭게 변환하세요.";
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
                    Changes = Array.Empty<string>(), // Could be enhanced to show specific changes
                    AppliedToneName = (tonePreset != null && tonePreset.Id != "default-none") ? tonePreset.Name : null
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

        public async Task<CommonQuestionResult> AnswerCommonQuestionAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return new CommonQuestionResult { Question = question, Answer = string.Empty };
            }

            try
            {
                var prompt = $"다음 질문에 간결하고 정확하게 답변해주세요. 답변은 한국어로 작성하되, 필요시 영어 용어를 병기해주세요.\n\n질문: {question}";

                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "system", content = "당신은 도움이 되는 AI 어시스턴트입니다. 사용자의 질문에 정확하고 유용한 답변을 제공합니다. 답변은 명확하고 이해하기 쉽게 작성하며, 필요한 경우 예시를 포함합니다." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = 1000
                };

                var response = await SendRequestAsync(requestBody);
                var answer = ExtractContentFromResponse(response);
                RecordUsageFromResponse(response, "CommonQuestion");

                return new CommonQuestionResult
                {
                    Question = question,
                    Answer = answer.Trim()
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Common question answering failed", ex);
            }
        }

        private async Task<string> SendRequestAsync(object requestBody)
        {
            _settings = _settingsService.LoadSettings(); // Reload settings for API key updates
            
            string provider = _settings.Provider ?? "OpenAI";
            string apiKey = _settings.GetApiKeyForProvider(provider);
            
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException($"API Key is not configured for {provider}. Please set your {provider} API key in settings.");
            }
            
            try
            {
                if (provider == "OpenAI")
                {
                    return await SendOpenAIRequestAsync(requestBody);
                }
                else if (provider == "Anthropic")
                {
                    return await SendAnthropicRequestAsync(requestBody);
                }
                else if (provider == "Gemini")
                {
                    return await SendGeminiRequestAsync(requestBody);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported provider: {provider}");
                }
            }
            catch (TaskCanceledException ex)
            {
                var timeoutSeconds = _settings.RequestTimeoutSeconds > 0 ? _settings.RequestTimeoutSeconds : 60;
                throw new TimeoutException($"Request timed out after {timeoutSeconds} seconds. The AI service did not respond in time.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Network error occurred while communicating with {provider}: {ex.Message}", ex);
            }
        }

        private async Task<string> SendOpenAIRequestAsync(object requestBody)
        {
            var apiKey = _settings.GetApiKeyForProvider("OpenAI");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.ApiEndpoint}/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
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

        private async Task<string> SendAnthropicRequestAsync(object requestBody)
        {
            // Convert OpenAI format to Anthropic format
            var openAIBody = JObject.FromObject(requestBody);
            var messages = openAIBody["messages"] as JArray;
            
            // Extract system message if present
            string systemMessage = "";
            var userMessages = new JArray();
            
            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    var role = msg["role"]?.ToString();
                    if (role == "system")
                    {
                        systemMessage = msg["content"]?.ToString() ?? "";
                    }
                    else
                    {
                        userMessages.Add(msg);
                    }
                }
            }

            var anthropicBody = new
            {
                model = _settings.Model,
                max_tokens = openAIBody["max_tokens"]?.Value<int>() ?? 2000,
                messages = userMessages,
                system = systemMessage,
                temperature = openAIBody["temperature"]?.Value<double>() ?? 0.3
            };

            var apiKey = _settings.GetApiKeyForProvider("Anthropic");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.ApiEndpoint}/messages");
            request.Headers.Add("x-api-key", apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = new StringContent(
                JsonConvert.SerializeObject(anthropicBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
            }

            // Convert Anthropic response to OpenAI format for compatibility
            var anthropicResponse = JObject.Parse(responseContent);
            var content = anthropicResponse["content"]?[0]?["text"]?.ToString() ?? "";
            
            var openAIResponse = new JObject
            {
                ["choices"] = new JArray
                {
                    new JObject
                    {
                        ["message"] = new JObject
                        {
                            ["content"] = content
                        }
                    }
                },
                ["usage"] = new JObject
                {
                    ["prompt_tokens"] = anthropicResponse["usage"]?["input_tokens"] ?? 0,
                    ["completion_tokens"] = anthropicResponse["usage"]?["output_tokens"] ?? 0,
                    ["total_tokens"] = (anthropicResponse["usage"]?["input_tokens"]?.Value<int>() ?? 0) + 
                                      (anthropicResponse["usage"]?["output_tokens"]?.Value<int>() ?? 0)
                }
            };

            return openAIResponse.ToString();
        }

        private async Task<string> SendGeminiRequestAsync(object requestBody)
        {
            // Convert OpenAI format to Gemini format
            var openAIBody = JObject.FromObject(requestBody);
            var messages = openAIBody["messages"] as JArray;
            
            // Build contents array for Gemini
            var contents = new JArray();
            string systemInstruction = "";
            
            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    var role = msg["role"]?.ToString();
                    var messageContent = msg["content"]?.ToString();
                    
                    if (role == "system")
                    {
                        systemInstruction = messageContent ?? "";
                    }
                    else
                    {
                        contents.Add(new JObject
                        {
                            ["role"] = role == "assistant" ? "model" : "user",
                            ["parts"] = new JArray
                            {
                                new JObject { ["text"] = messageContent }
                            }
                        });
                    }
                }
            }

            var geminiBody = new JObject
            {
                ["contents"] = contents,
                ["generationConfig"] = new JObject
                {
                    ["temperature"] = openAIBody["temperature"]?.Value<double>() ?? 0.3,
                    ["maxOutputTokens"] = openAIBody["max_tokens"]?.Value<int>() ?? 2000
                }
            };

            if (!string.IsNullOrEmpty(systemInstruction))
            {
                geminiBody["systemInstruction"] = new JObject
                {
                    ["parts"] = new JArray { new JObject { ["text"] = systemInstruction } }
                };
            }

            var apiKey = _settings.GetApiKeyForProvider("Gemini");
            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"{_settings.ApiEndpoint}/models/{_settings.Model}:generateContent?key={apiKey}");
            request.Content = new StringContent(
                geminiBody.ToString(),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
            }

            // Convert Gemini response to OpenAI format for compatibility
            var geminiResponse = JObject.Parse(responseContent);
            var responseText = geminiResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";
            
            var promptTokens = geminiResponse["usageMetadata"]?["promptTokenCount"]?.Value<int>() ?? 0;
            var completionTokens = geminiResponse["usageMetadata"]?["candidatesTokenCount"]?.Value<int>() ?? 0;
            
            var openAIResponse = new JObject
            {
                ["choices"] = new JArray
                {
                    new JObject
                    {
                        ["message"] = new JObject
                        {
                            ["content"] = responseText
                        }
                    }
                },
                ["usage"] = new JObject
                {
                    ["prompt_tokens"] = promptTokens,
                    ["completion_tokens"] = completionTokens,
                    ["total_tokens"] = promptTokens + completionTokens
                }
            };

            return openAIResponse.ToString();
        }

        private string ExtractContentFromResponse(string response)
        {
            try
            {
                var json = JObject.Parse(response);
                var content = json["choices"]?[0]?["message"]?["content"]?.ToString();
                return content ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Return empty string if response format is unexpected
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to extract content: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
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
            catch (Exception ex)
            {
                // Silently fail - don't interrupt user operations
                // Usage tracking is non-critical feature
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to record usage: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
            }
        }
    }
}
