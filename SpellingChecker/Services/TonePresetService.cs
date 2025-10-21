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
                    Id = "default-eager-newbie",
                    Name = "싹싹한 신입 사원 톤",
                    Description = "해맑고 공손하며 적극적인 태도의 말투.",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-cat",
                    Name = "고양이",
                    Description = "야옹채로 응답",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-friendly",
                    Name = "\"~어요\" ",
                    Description = "\"~했습니다\" 대신 \"~했어요\"로 친근감있게 표현",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-ieungBatchim",
                    Name = "단어에 이응 받침",
                    Description = "모든 단어에 이응 받침을 붙인다",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-shyDisposition",
                    Name = "소심",
                    Description = "매우 소심해서 지나치게 주저하며 말하는 7세 아이 말투",
                    IsDefault = true
                },
                new TonePreset
                {
                    Id = "default-exclamationMark",
                    Name = "문장 끝에 느낌표",
                    Description = "문장 끝에 항상 느낌표를 붙인다",
                    IsDefault = true
                },
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
