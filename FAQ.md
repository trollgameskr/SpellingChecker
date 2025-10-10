# Frequently Asked Questions (FAQ)

## General Questions

### What is SpellingChecker?

SpellingChecker is a Windows desktop application that provides AI-powered spelling correction and translation between Korean and English. It runs in the background and can be activated with global hotkeys.

### What makes it different from other spell checkers?

- **AI-Powered**: Uses advanced GPT models for context-aware corrections
- **System-Wide**: Works in any application (Word, browser, email, etc.)
- **Translation**: Instant Korean ↔ English translation
- **Lightweight**: Runs in background with minimal resource usage
- **Privacy-Focused**: No data collection, encrypted settings

### Is it free?

The application itself is free and open-source (MIT License). However, you need an OpenAI API key, which is a paid service. Typical usage costs $1-5 per month for moderate use.

## Installation & Setup

### What are the system requirements?

- Windows 10 or later (64-bit)
- .NET 9.0 Runtime (included with installer)
- Internet connection
- OpenAI API key

### How do I get an OpenAI API key?

1. Visit https://platform.openai.com/
2. Create an account or sign in
3. Go to API Keys section
4. Click "Create new secret key"
5. Copy the key and paste it into SpellingChecker settings

### How much does the OpenAI API cost?

For the recommended `gpt-4o-mini` model:
- Input: $0.15 per 1M tokens (~750k words)
- Output: $0.60 per 1M tokens

**Practical costs**:
- 100 corrections: ~$0.01
- Daily use (50 corrections/day): ~$0.50-1.00/month
- Heavy use: ~$3-5/month

Check current pricing at https://openai.com/pricing

### Where are my settings stored?

Settings are encrypted and stored at:
```
%APPDATA%\SpellingChecker\settings.json
```

This translates to:
```
C:\Users\YourUsername\AppData\Roaming\SpellingChecker\settings.json
```

### Is my API key secure?

Yes! Your API key is encrypted using Windows Data Protection API (DPAPI) with the following security measures:
- Encrypted at rest (not plain text)
- User-specific encryption (other users can't access)
- Protected by Windows security
- Never transmitted except to OpenAI's API

## Using the Application

### How do I use spelling correction?

1. Select text in any application
2. Press **Ctrl+Shift+Y**
3. Wait for the popup (usually 1-3 seconds)
4. Click **Copy to Clipboard** or **Replace**

### How do I use translation?

1. Select text in any application
2. Press **Ctrl+Shift+T**
3. The app detects the language automatically
4. Review the translation in the popup
5. Click **Copy to Clipboard** or **Replace**

### Can I change the hotkeys?

Not in version 1.0. Customizable hotkeys are planned for a future release. Currently:
- **Ctrl+Shift+Y**: Spelling correction
- **Ctrl+Shift+T**: Translation

### The hotkeys don't work. What should I do?

**Common solutions**:
1. Check if another app is using the same hotkeys
2. Try running SpellingChecker as Administrator
3. Restart the application
4. Check if the app is running (system tray icon should be visible)

### Why does it say "No text selected"?

This happens when:
- No text was selected before pressing the hotkey
- The application doesn't support text selection copy
- Clipboard access is blocked

**Solution**: Make sure to select (highlight) the text before pressing the hotkey.

### How accurate is the spelling correction?

The AI model (gpt-4o-mini by default) is highly accurate for:
- Spelling errors
- Grammar mistakes
- Punctuation
- Common typos

However, it may occasionally:
- Change writing style slightly
- Misinterpret intentional informal language
- Make unnecessary changes to already correct text

Always review the suggestions!

### Which languages are supported?

Currently:
- **Korean**: Full support for spelling correction and translation
- **English**: Full support for spelling correction and translation

**Future versions** may include:
- Japanese
- Chinese
- Spanish
- French
- German

### Can I use it offline?

No, version 1.0 requires an internet connection to reach the OpenAI API. Offline mode with local models is planned for future releases.

## Troubleshooting

### "API Key is not configured"

**Cause**: API key hasn't been entered in settings.

**Solution**:
1. Double-click the system tray icon
2. Enter your OpenAI API key
3. Click Save

### "API request failed: 401 Unauthorized"

**Cause**: Invalid or expired API key.

**Solutions**:
1. Verify your API key is correct
2. Check if your OpenAI account is active
3. Generate a new API key at https://platform.openai.com/api-keys

### "API request failed: 429 Too Many Requests"

**Cause**: Rate limit exceeded or insufficient credits.

**Solutions**:
1. Wait a few minutes and try again
2. Check your OpenAI account billing at https://platform.openai.com/account/billing
3. Add credits to your account if needed

### "API request failed: timeout"

**Cause**: Slow internet or API response.

**Solutions**:
1. Check your internet connection
2. Try again (may be temporary server load)
3. Use a faster AI model (gpt-3.5-turbo)

### The app doesn't start

**Solutions**:
1. Check if .NET 9.0 is installed
2. Look for errors in Windows Event Viewer
3. Try reinstalling the application
4. Run from command line to see error messages

### The result popup appears in the wrong location

**Cause**: Multi-monitor setup or display scaling.

**Solution**: This is a known limitation. The popup tries to appear near the cursor but may need adjustment on some setups. Future versions will improve multi-monitor support.

### Copy/Replace doesn't work

**Possible causes**:
1. Target application doesn't allow clipboard paste
2. Security software blocking clipboard access
3. Application running without sufficient permissions

**Solutions**:
1. Try running as Administrator
2. Check security software settings
3. Use Copy button and paste manually (Ctrl+V)

## Features & Functionality

### Can it correct entire documents?

The app is designed for quick corrections of selected text passages. For entire documents:
1. Select sections one at a time
2. Correct each section
3. Or copy entire document to a text editor and process in chunks

Large documents may:
- Take longer to process
- Cost more API credits
- Hit API token limits

### Does it work in Microsoft Word?

Yes! Select text in Word and use the hotkeys. However, Word has its own built-in spell checker that you may prefer for basic corrections.

### Does it work in web browsers?

Yes! Works in Chrome, Edge, Firefox, and other browsers. Select text on any webpage and use the hotkeys.

### Can I use it for emails?

Absolutely! Works in Outlook, Gmail (browser), and other email clients. Perfect for:
- Writing professional emails
- Translating correspondence
- Checking grammar before sending

### What AI model should I use?

**Recommended: gpt-4o-mini**
- Fast (1-3 seconds)
- Cost-effective
- Good quality

**Alternative: gpt-4o**
- Slower (3-5 seconds)
- More expensive
- Highest quality
- Best for important documents

**Budget option: gpt-3.5-turbo**
- Fastest (<1 second)
- Cheapest
- Lower quality

### Can I use a different AI provider?

Version 1.0 only supports OpenAI. Future versions may support:
- Azure OpenAI
- Anthropic Claude
- Local models (Llama, etc.)

## Privacy & Security

### What data is sent to OpenAI?

Only:
- The text you select and process
- System prompts (instructions to the AI)
- API authentication

**Not sent**:
- Other text in your documents
- Your computer information
- Browsing history or activity

### Does the app collect any data about me?

No! The app:
- ✅ Does not collect telemetry
- ✅ Does not track usage
- ✅ Does not send analytics
- ✅ Does not store your text

### What does OpenAI do with my data?

According to OpenAI's policy (as of 2024):
- API requests are **not** used to train models
- Data may be retained for 30 days for abuse monitoring
- Review their current policy: https://openai.com/policies/api-data-usage-policies

### Can my employer see what I'm correcting?

If you're on a corporate network:
- HTTPS encryption protects the content
- Network admins can see you're accessing OpenAI
- They cannot see the actual text being processed
- Check your company's IT policies

### Should I use it for sensitive/confidential information?

**Consider**:
- The text is sent to OpenAI's servers
- OpenAI has security measures but it's still external
- For highly sensitive info (trade secrets, personal data), consider:
  - Using offline tools
  - Waiting for offline mode in future versions
  - Company-approved tools only

## Advanced Usage

### Can I use my own API endpoint?

Yes! In Settings, change the "API Endpoint" to:
- Azure OpenAI: `https://your-resource.openai.azure.com/`
- Custom proxy: `http://localhost:8080/v1`
- Other OpenAI-compatible endpoints

### Can I run it on multiple computers?

Yes! Install on each computer and use your API key on all of them. Be aware:
- API costs apply to all requests
- Rate limits are per API key
- Settings are per-computer (not synced)

### How do I set it to start with Windows?

1. Open Settings (double-click tray icon)
2. Check "Start with Windows"
3. Click Save

The app will now start automatically when you log in to Windows.

### Can I use it for programming/code?

Yes, but:
- It may change code formatting
- It might "correct" intentional abbreviations
- Better for comments and documentation
- Not ideal for code syntax

**Tip**: Select only comments or documentation for correction.

## Updates & Support

### How do I update the app?

Currently: Download and install the latest version from GitHub Releases.

**Future**: Auto-update feature is planned.

### Where can I report bugs?

- GitHub Issues: https://github.com/shinepcs/SpellingChecker/issues
- Include: OS version, app version, steps to reproduce

### Where can I request features?

- GitHub Issues with "enhancement" label
- Describe your use case and why it's useful

### How can I contribute?

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on:
- Reporting bugs
- Suggesting features
- Contributing code
- Improving documentation

### Is there a roadmap?

Check the [CHANGELOG.md](CHANGELOG.md) for planned features. Upcoming priorities:
- Customizable hotkeys
- Auto-update mechanism
- Additional language support
- Offline mode

## Troubleshooting Common Scenarios

### Scenario: "I selected text but it says no text selected"

**Diagnosis**:
1. Is the text actually highlighted?
2. Does the application support copy (Ctrl+C)?
3. Try copying manually first to test

**Solutions**:
- Some apps (PDFs, images) don't support text selection
- Try a different application
- Run as Administrator

### Scenario: "The correction changed my text in unexpected ways"

**Explanation**: The AI tries to improve text but may:
- Change writing style
- Rephrase for clarity
- Over-correct casual language

**Solutions**:
- Review before accepting
- Use "Copy" instead of "Replace" to preview
- Select smaller chunks of text
- Adjust the prompt (future feature)

### Scenario: "It's too slow"

**Causes**:
- Slow internet connection
- OpenAI server load
- Large text selection
- Using slower model (gpt-4o)

**Solutions**:
- Switch to gpt-4o-mini or gpt-3.5-turbo
- Select smaller text chunks
- Check internet speed
- Try at different time of day

### Scenario: "It's costing too much"

**Cost optimization**:
- Use gpt-4o-mini instead of gpt-4o (4x cheaper)
- Select only necessary text
- Don't correct already correct text
- Monitor usage at https://platform.openai.com/usage

## Getting Help

Still have questions?

1. **Documentation**: Check [README.md](README.md), [CONFIG.md](CONFIG.md)
2. **Issues**: Search existing issues on GitHub
3. **New Question**: Open a discussion on GitHub
4. **Bug Report**: Create an issue with details

---

**Didn't find your answer?** Open an issue at https://github.com/shinepcs/SpellingChecker/issues
