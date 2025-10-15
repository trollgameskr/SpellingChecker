# Quick Start Guide

Get started with AI Spelling Checker in 5 minutes!

## Prerequisites

- Windows 10 or later
- .NET 9.0 Runtime (will be installed automatically with the installer)
- API Key from one of the supported providers:
  - [OpenAI](https://platform.openai.com/api-keys)
  - [Anthropic](https://console.anthropic.com/)
  - [Google Gemini](https://makersuite.google.com/app/apikey)

## Installation

### Option 1: Download Release (Recommended)

1. Go to [Releases](https://github.com/shinepcs/SpellingChecker/releases)
2. Download the latest `SpellingCheckerSetup_vX.X.X.exe`
3. Run the installer
4. Follow the installation wizard

### Option 2: Build from Source

See [BUILD.md](BUILD.md) for detailed instructions.

## First-Time Setup

### 1. Get an API Key

Choose one of the supported AI providers:

#### OpenAI
1. Visit https://platform.openai.com/
2. Sign up or log in
3. Go to **API Keys** section
4. Click **Create new secret key**
5. Copy the key (starts with `sk-...`)

#### Anthropic
1. Visit https://console.anthropic.com/
2. Sign up or log in
3. Go to **API Keys** section
4. Click **Create Key**
5. Copy the key (starts with `sk-ant-...`)

#### Google Gemini
1. Visit https://makersuite.google.com/app/apikey
2. Sign up or log in with your Google account
3. Click **Create API Key**
4. Copy the key

### 2. Configure the Application

1. After installation, the app will start automatically and appear in the system tray (bottom-right corner of your screen)

2. **Open Settings**:
   - Double-click the system tray icon, OR
   - Right-click the icon → **Settings**

3. **Select your AI Provider**:
   - Choose from OpenAI, Anthropic, or Gemini
   - The API endpoint will be set automatically

4. **Enter your API Key**:
   - Paste your API key for the selected provider

5. **Select a Model** (optional):
   - The default model is recommended for most users
   - You can choose a different model based on your needs

6. **Click Save**

That's it! You're ready to use the app.

## Using the App

### Spelling Correction

1. **Select text** anywhere (Word, Notepad, Chrome, etc.)
2. Press **Ctrl+Shift+Alt+Y**
3. A notification will appear: "Processing..." - wait a moment for the AI to process
4. A popup will show:
   - **Original**: Your text
   - **Result**: Corrected text
5. Click **Copy to Clipboard** or **Replace** to use the correction

#### Example:
```
Original: "Ths is a tst sentance with erors."
Result:   "This is a test sentence with errors."
```

### Translation

1. **Select Korean or English text**
2. Press **Ctrl+Shift+Alt+T**
3. A notification will appear: "Processing..." - wait a moment for the AI to translate
4. A popup will show:
   - **Original**: Your text
   - **Result**: Translated text
   - Language direction (e.g., "Korean → English")
5. Click **Copy to Clipboard** or **Replace**

#### Examples:

**Korean to English**:
```
Original: "안녕하세요, 반갑습니다."
Result:   "Hello, nice to meet you."
```

**English to Korean**:
```
Original: "How are you doing today?"
Result:   "오늘 어떻게 지내세요?"
```

## Tips & Tricks

### 1. Best Practices

- **Select only the text you need**: Shorter text = faster response
- **Check your internet connection**: The app needs internet to reach OpenAI
- **Keep the app running**: It runs in the background, using minimal resources

### 2. Common Use Cases

**Writing Emails**:
1. Write your email in Korean
2. Select the text
3. Press your translation hotkey (default: Ctrl+Shift+Alt+T) to translate to English
4. Press your spelling correction hotkey (default: Ctrl+Shift+Alt+Y) to correct any mistakes
5. Copy and send!

**Editing Documents**:
1. Write your document
2. Select paragraphs one at a time
3. Press your spelling correction hotkey (default: Ctrl+Shift+Alt+Y) to correct
4. Review and replace

**Learning Languages**:
1. Write in your target language
2. Press your spelling correction hotkey (default: Ctrl+Shift+Alt+Y) to see corrections
3. Compare original and corrected versions

### 3. Keyboard Shortcuts

The default hotkeys are:

| Shortcut | Function |
|----------|----------|
| Ctrl+Shift+Alt+Y | Spelling correction |
| Ctrl+Shift+Alt+T | Translation |

**You can customize these hotkeys!**
1. Open Settings (double-click tray icon)
2. Scroll to the "Hotkeys" section
3. Enter your preferred hotkey combinations
4. Click Save and restart the application

### 4. System Tray Menu

Right-click the system tray icon:
- **Settings**: Open settings window
- **Exit**: Close the application

## Troubleshooting

### "API Key is not configured"

**Solution**: Open Settings and enter your API key for your selected provider (OpenAI, Anthropic, or Gemini)

### "No text selected"

**Solution**: Make sure you've selected text before pressing the hotkey

### "API request failed"

**Solutions**:
- Check your internet connection
- Verify your API key is valid
- Check if you have API credits for your provider (OpenAI, Anthropic, or Gemini)
- Ensure the selected model is available for your API key

### Hotkeys don't work

**Solutions**:
- Check if another app is using the same hotkeys
- Try running as administrator
- Restart the application

### App doesn't start

**Solutions**:
- Make sure .NET 9.0 is installed
- Check Windows Event Viewer for errors
- Try reinstalling

## Advanced Features

### Hotkey Customization

Customize your hotkeys to your preference:
1. Open Settings
2. In the "Hotkeys" section, enter your preferred combinations
3. Supported formats:
   - `Ctrl+Y` (single modifier + key)
   - `Ctrl+Shift+Y` (multiple modifiers + key)
   - `Alt+F1` (modifier + function key)
4. Click Save
5. Restart the application

### Auto-Start with Windows

1. Open Settings
2. Check "Start with Windows"
3. Click Save
4. The app will now start automatically when you log in

### Custom API Endpoint

For advanced users or organizations:
1. Open Settings
2. Change "API Endpoint" to your custom URL
3. Save

### Different AI Models

For different quality/speed trade-offs:
1. Open Settings
2. Select your preferred AI Provider (OpenAI, Anthropic, or Gemini)
3. Choose a Model from the dropdown:
   - **OpenAI**: gpt-4o-mini (fast, recommended), gpt-4o (highest quality), gpt-3.5-turbo (fastest)
   - **Anthropic**: claude-3-5-sonnet (balanced, recommended), claude-3-5-haiku (fast), claude-3-opus (highest quality)
   - **Gemini**: gemini-2.0-flash-exp (latest, recommended), gemini-1.5-pro (high quality), gemini-1.5-flash (fast)
4. Save

## Cost Information

OpenAI charges for API usage:

**Typical costs with gpt-4o-mini**:
- 100 corrections: ~$0.01
- 1000 corrections: ~$0.10
- Heavy daily use: ~$1-5 per month

See https://openai.com/pricing for current rates.

## Privacy

- Your text is sent to OpenAI for processing
- API key is encrypted and stored locally
- No data is stored by this application
- No telemetry or usage tracking

## Getting Help

- **Documentation**: Check [CONFIG.md](CONFIG.md) for detailed configuration
- **Issues**: Report bugs at https://github.com/shinepcs/SpellingChecker/issues
- **FAQ**: See [README.md](README.md)

## Next Steps

- Explore different AI models for your needs
- Set up auto-start for convenience
- Share the app with colleagues
- Contribute to the project on GitHub!

---

**Enjoy using AI Spelling Checker!** 🎉

For more information, see the full [README.md](README.md) and [documentation](https://github.com/shinepcs/SpellingChecker).
