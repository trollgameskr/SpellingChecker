using System;
using System.Collections.Generic;
using System.Linq;
using SpellingChecker.Models;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for managing tone presets
    /// </summary>
    public class TonePresetService
    {
        /// <summary>
        /// Get the default tone presets
        /// </summary>
        public static List<TonePreset> GetDefaultTonePresets()
        {
            return new List<TonePreset>
            {
                new TonePreset
                {
                    Id = "default-none",
                    Name = "톤 없음",
                    Description = "원문의 톤을 그대로 유지합니다.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-strict-boss",
                    Name = "근엄한 팀장님 톤",
                    Description = "권위 있고 엄격한 말투, 지시와 조언이 섞인 느낌.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-eager-newbie",
                    Name = "싹싹한 신입 사원 톤",
                    Description = "해맑고 공손하며 적극적인 태도의 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-mz-gen",
                    Name = "MZ세대 톤",
                    Description = "최신 유행어와 인터넷 밈을 섞은 가벼운 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-bored-parttime",
                    Name = "심드렁한 알바생 톤",
                    Description = "의욕 없는 듯한 무심한 말투, 최소한의 답변 느낌.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-polite-guard",
                    Name = "유난히 예의 바른 경비원 톤",
                    Description = "과도하게 공손하고 절차를 강조하는 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-excited-host",
                    Name = "오버하는 홈쇼핑 쇼호스트 톤",
                    Description = "지나치게 흥분하며 모든 것을 최고라 강조하는 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-comedian",
                    Name = "유행어 난발하는 예능인 톤",
                    Description = "빠른 말속에 웃긴 상황과 유행어를 연속적으로 넣는 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-grandma",
                    Name = "100년 된 할머니 톤",
                    Description = "옛스러운 단어와 느릿한 말투, 추억 섞인 문장.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-chaebol",
                    Name = "드라마 재벌 2세 톤",
                    Description = "거만하고 사치스러운 분위기의 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-foreigner",
                    Name = "외국인 한국어 학습자 톤",
                    Description = "문법이 조금 서툴고 귀여운 표현이 섞인 말투.",
                    IsDefault = true
                }
            };
        }

        /// <summary>
        /// Initialize tone presets if they don't exist
        /// </summary>
        public static void InitializeTonePresets(AppSettings settings)
        {
            if (settings.TonePresets == null || settings.TonePresets.Count == 0)
            {
                settings.TonePresets = GetDefaultTonePresets();
                settings.SelectedTonePresetId = "default-none";
            }
        }

        /// <summary>
        /// Get the selected tone preset
        /// </summary>
        public static TonePreset? GetSelectedTonePreset(AppSettings settings)
        {
            if (string.IsNullOrEmpty(settings.SelectedTonePresetId))
            {
                return null;
            }

            return settings.TonePresets?.FirstOrDefault(p => p.Id == settings.SelectedTonePresetId);
        }

        /// <summary>
        /// Add a new custom tone preset
        /// </summary>
        public static void AddTonePreset(AppSettings settings, string name, string description)
        {
            if (settings.TonePresets == null)
            {
                settings.TonePresets = new List<TonePreset>();
            }

            var preset = new TonePreset
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                IsDefault = false
            };

            settings.TonePresets.Add(preset);
        }

        /// <summary>
        /// Update an existing tone preset
        /// </summary>
        public static bool UpdateTonePreset(AppSettings settings, string id, string name, string description)
        {
            var preset = settings.TonePresets?.FirstOrDefault(p => p.Id == id);
            if (preset == null)
            {
                return false;
            }

            preset.Name = name;
            preset.Description = description;
            return true;
        }

        /// <summary>
        /// Delete a tone preset
        /// </summary>
        public static bool DeleteTonePreset(AppSettings settings, string id)
        {
            var preset = settings.TonePresets?.FirstOrDefault(p => p.Id == id);
            if (preset == null)
            {
                return false;
            }

            settings.TonePresets?.Remove(preset);
            
            // If the deleted preset was selected, reset to default
            if (settings.SelectedTonePresetId == id)
            {
                settings.SelectedTonePresetId = "default-none";
            }

            return true;
        }
    }
}
