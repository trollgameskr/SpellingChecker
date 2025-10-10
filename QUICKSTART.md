# Quick Start Guide

Get started with AI Spelling Checker in 5 minutes!

## Prerequisites

- Windows 10 or later
- .NET 9.0 Runtime (will be installed automatically with the installer)
- OpenAI API Key ([Get one here](https://platform.openai.com/api-keys))

## Installation

### Option 1: Download Release (Recommended)

1. Go to [Releases](https://github.com/shinepcs/SpellingChecker/releases)
2. Download the latest `SpellingCheckerSetup_vX.X.X.exe`
3. Run the installer
4. Follow the installation wizard

### Option 2: Build from Source

See [BUILD.md](BUILD.md) for detailed instructions.

## First-Time Setup

### 1. Get an OpenAI API Key

1. Visit https://platform.openai.com/
2. Sign up or log in
3. Go to **API Keys** section
4. Click **Create new secret key**
5. Copy the key (starts with `sk-...`)

### 2. Configure the Application

1. After installation, the app will start automatically and appear in the system tray (bottom-right corner of your screen)

2. **Open Settings**:
   - Double-click the system tray icon, OR
   - Right-click the icon → **Settings**

3. **Enter your API Key**:
   - Paste your OpenAI API key
   - The default settings are fine for most users

4. **Click Save**

That's it! You're ready to use the app.

## Using the App

### Spelling Correction

1. **Select text** anywhere (Word, Notepad, Chrome, etc.)
2. Press **Ctrl+Shift+Y**
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
2. Press **Ctrl+Shift+T**
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
3. Press Ctrl+Shift+T to translate to English
4. Press Ctrl+Shift+Y to correct any mistakes
5. Copy and send!

**Editing Documents**:
1. Write your document
2. Select paragraphs one at a time
3. Press Ctrl+Shift+Y to correct
4. Review and replace

**Learning Languages**:
1. Write in your target language
2. Press Ctrl+Shift+Y to see corrections
3. Compare original and corrected versions

### 3. Keyboard Shortcuts

| Shortcut | Function |
|----------|----------|
| Ctrl+Shift+Y | Spelling correction |
| Ctrl+Shift+T | Translation |

### 4. System Tray Menu

Right-click the system tray icon:
- **Settings**: Open settings window
- **Exit**: Close the application

## Troubleshooting

### "API Key is not configured"

**Solution**: Open Settings and enter your OpenAI API key

### "No text selected"

**Solution**: Make sure you've selected text before pressing the hotkey

### "API request failed"

**Solutions**:
- Check your internet connection
- Verify your API key is valid
- Check if you have OpenAI API credits

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
2. Change "AI Model":
   - `gpt-4o-mini`: Fast, balanced (default)
   - `gpt-4o`: Slower, highest quality
   - `gpt-3.5-turbo`: Fastest, lower quality
3. Save

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
