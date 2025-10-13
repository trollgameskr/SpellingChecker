# Tone Preset Feature - Quick Reference

## What's New?

The SpellingChecker now supports **문장 톤 프리셋 (Tone Presets)** - apply different speaking styles to your corrected text!

## 🎯 Key Features

### ✨ 11 Default Tone Presets
1. **톤 없음** - No tone (keeps original style)
2. **근엄한 팀장님 톤** - Strict manager tone
3. **싹싹한 신입 사원 톤** - Eager new employee tone
4. **MZ세대 톤** - MZ generation slang
5. **심드렁한 알바생 톤** - Bored part-timer tone
6. **유난히 예의 바른 경비원 톤** - Overly polite guard
7. **오버하는 홈쇼핑 쇼호스트 톤** - Excited TV host
8. **유행어 난발하는 예능인 톤** - Comedian with trendy phrases
9. **100년 된 할머니 톤** - 100-year-old grandmother
10. **드라마 재벌 2세 톤** - Drama chaebol heir
11. **외국인 한국어 학습자 톤** - Korean learner

### 🛠️ Custom Tone Management
- ➕ **Add** your own custom tones
- ✏️ **Edit** custom tones
- 🗑️ **Delete** custom tones
- 🔒 Default tones are protected

## 📖 How to Use

### Select a Tone Preset
1. Open Settings (tray icon → Settings)
2. Find "문장 톤 프리셋" section
3. Select your desired tone from dropdown
4. Click Save
5. Use spelling correction as usual (Ctrl+Shift+Alt+Y)

### Create Custom Tone
1. Settings → "문장 톤 프리셋" section
2. Click "➕ 추가" button
3. Enter tone name (e.g., "공식 보고서 톤")
4. Enter description (e.g., "격식 있고 전문적인 말투")
5. Click "확인"

### Edit Custom Tone
1. Select a custom tone from dropdown
2. Click "✏️ 수정" button
3. Modify name or description
4. Click "확인"

### Delete Custom Tone
1. Select a custom tone
2. Click "🗑️ 삭제" button
3. Confirm deletion

## 💡 Example Use Cases

### Business Email
**Tone**: "공식 이메일 톤" (custom)
**Input**: "회의 자료 보냈습니다"
**Output**: "회의 자료를 송부하였습니다. 검토 부탁드립니다."

### Friendly Chat
**Tone**: "MZ세대 톤"
**Input**: "회의 자료 보냈습니다"
**Output**: "회의 자료 보냈어~ㅋㅋ 확인해봐 ㄱㄱ"

### Formal Report
**Tone**: "근엄한 팀장님 톤"
**Input**: "프로젝트 진행 상황 보고합니다"
**Output**: "프로젝트 진행 상황을 보고드립니다. 현재까지 순조롭게 진행되고 있으니 확인하시기 바랍니다."

## 🔧 Technical Details

### New Files
- `TonePresetService.cs` - Manages tone presets
- `TonePresetDialog.xaml/cs` - Add/Edit dialog

### Modified Files
- `Models.cs` - Added TonePreset model
- `AIService.cs` - Applies tone during correction
- `SettingsService.cs` - Initializes default presets
- `SettingsWindow.xaml/cs` - UI and logic for tone management

### Data Storage
- Tone presets stored in `%APPDATA%\SpellingChecker\settings.json`
- Encrypted with Windows DPAPI
- Persists across application restarts

## 📋 Settings Window Changes

### New UI Section
```
문장 톤 프리셋:
┌────────────────────────┐
│ 톤 없음           ▼   │
└────────────────────────┘
원문의 톤을 그대로 유지합니다.

[➕ 추가] [✏️ 수정] [🗑️ 삭제]
```

### Window Size
- Height: 700px → **800px**
- Width: 600px → **650px**

## ⚙️ How It Works

1. **User selects tone** in Settings
2. **Tone is saved** to encrypted settings file
3. **During spelling correction**:
   - AIService retrieves selected tone
   - If tone is not "톤 없음", modifies AI prompt
   - AI corrects text AND applies tone
   - Result is displayed in popup

## 🔐 Security & Privacy

- ✅ Tone presets encrypted with other settings
- ✅ No data sent to external servers (only OpenAI API)
- ✅ User-specific (per Windows user account)
- ✅ Default presets cannot be modified

## 📚 Documentation

For more detailed information, see:
- `TONE_PRESET_IMPLEMENTATION.md` - Full implementation details
- `TONE_PRESET_TESTING.md` - Comprehensive testing guide
- `TONE_PRESET_UI_GUIDE.md` - Visual UI guide
- `CONFIG.md` - Configuration guide (includes tone presets section)
- `README.md` - Updated with tone preset features

## 🐛 Troubleshooting

### Tone not applying?
- Check that tone is selected in Settings (not "톤 없음")
- Save settings after selecting tone
- Verify API key is configured correctly

### Can't edit/delete tone?
- Default tones cannot be edited or deleted
- Only custom tones can be modified
- Edit/Delete buttons are disabled for default tones

### Tone presets missing after restart?
- Check settings file: `%APPDATA%\SpellingChecker\settings.json`
- If corrupted, delete file and restart (defaults will reload)
- Custom tones may be lost if settings file is corrupted

## ✅ Quality Assurance

### Testing Completed
- ✅ All 11 default tones load correctly
- ✅ Custom tone add/edit/delete works
- ✅ Tone applies during spelling correction
- ✅ Settings persist after restart
- ✅ UI updates correctly
- ✅ Validation prevents empty inputs
- ✅ Default tones are protected

### Backward Compatibility
- ✅ Existing functionality unchanged
- ✅ Old settings files automatically upgraded
- ✅ Translation feature unaffected
- ✅ Hotkeys still customizable

## 🚀 Future Enhancements

Potential improvements for future versions:
- Tone preview with examples
- Tone intensity slider
- Import/Export custom presets
- AI-suggested tones based on context
- Multilingual tone support
- Tone analytics and usage tracking

## 📝 Quick Commands

```bash
# Build project
cd SpellingChecker
dotnet build

# Run application
dotnet run

# Settings location
%APPDATA%\SpellingChecker\settings.json
```

## 📞 Support

For issues or questions:
1. Check documentation files
2. Review testing guide: `TONE_PRESET_TESTING.md`
3. Open GitHub issue with details
4. Include steps to reproduce any problems

---

**Version**: 1.0.0 (Tone Preset Feature)
**Last Updated**: 2025-10-13
**Status**: ✅ Production Ready
