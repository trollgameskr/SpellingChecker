# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-10

### Added
- Initial release of AI Spelling Checker and Translation application
- AI-powered spelling and grammar correction using OpenAI GPT models
- Automatic language detection for Korean and English
- Korean-to-English and English-to-Korean translation
- Global hotkeys:
  - Ctrl+Shift+Alt+Y for spelling correction
  - Ctrl+Shift+Alt+T for translation
- System tray integration
- Background operation mode
- Settings window for API configuration
- Secure API key storage using Windows Data Protection API
- Copy to clipboard functionality
- Text replacement functionality
- Popup results window with original and corrected/translated text
- Configuration guide and build documentation
- MIT License

### Features
- **Spelling Correction**: Select any text in any application and press Ctrl+Shift+Alt+Y to get AI-powered corrections
- **Translation**: Select text and press Ctrl+Shift+Alt+T to translate between Korean and English
- **Privacy**: All settings encrypted locally, no data stored
- **Performance**: Uses gpt-4o-mini model for fast responses
- **User Experience**: Intuitive popup UI, easy copy and replace functions

### Security
- API keys encrypted using Windows DPAPI
- No telemetry or data collection
- All processing happens via secure HTTPS to OpenAI

## [Unreleased]

### Added
- Usage statistics and history tracking
  - Track token usage (prompt and completion tokens)
  - Calculate costs based on model pricing
  - View detailed usage history with date/time, operation type, and costs
  - Filter statistics by period (today, this week, this month, all time)
  - Clear usage history option
  - Accessible from Settings window
- **Change highlighting**: Spelling corrections now highlight changed words in yellow for easy identification of modifications

### Planned Features
- Customizable hotkeys
- Support for additional languages (Japanese, Chinese, Spanish, etc.)
- Offline model support
- Change highlighting in correction results
- Usage statistics and history
- Automatic updates
- Dark mode theme
- Custom dictionary support
- Context menu integration
- Browser extension support

### Known Limitations
- Windows 10 or later required
- Requires .NET 9.0 runtime
- Requires OpenAI API key (paid service)
- Some applications may block clipboard access
- Fixed hotkeys (customization coming in future version)

## Version History

- **1.0.0** - Initial release (January 2025)
