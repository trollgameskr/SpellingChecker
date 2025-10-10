# Configuration Guide

## Getting an OpenAI API Key

1. Visit https://platform.openai.com/
2. Sign up or log in to your account
3. Navigate to API Keys section
4. Click "Create new secret key"
5. Copy the key (you won't be able to see it again)
6. Paste it into the SpellingChecker settings

## API Key Security

Your API key is:
- Encrypted using Windows Data Protection API (DPAPI)
- Stored per-user in `%APPDATA%\SpellingChecker\settings.json`
- Never transmitted anywhere except OpenAI's API
- Protected from access by other users on the same computer

## Configuration Options

### API Endpoint

**Default**: `https://api.openai.com/v1`

You can change this to:
- Use a different OpenAI-compatible API
- Use a proxy server
- Use a local API endpoint for testing

### AI Model

**Default**: `gpt-4o-mini`

Available models:
- `gpt-4o-mini`: Fast, cost-effective, good quality (recommended)
- `gpt-4o`: Slower, more expensive, highest quality
- `gpt-3.5-turbo`: Faster, less expensive, lower quality

**Recommendation**: Use `gpt-4o-mini` for the best balance of speed, cost, and quality.

### Auto Start with Windows

When enabled, the application will:
- Start automatically when Windows boots
- Run in the background (system tray)
- Be ready to use immediately

**Registry Key** (for auto-start):
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
Value: SpellingChecker
Data: "C:\Program Files\SpellingChecker\SpellingChecker.exe"
```

## Hotkey Customization

You can customize hotkeys through the settings window:

**Default hotkeys:**
- **Ctrl+Shift+Alt+Y**: Spelling correction
- **Ctrl+Shift+Alt+T**: Translation

**To customize hotkeys:**

1. Open Settings (double-click tray icon or right-click → Settings)
2. In the "Hotkeys" section, modify the hotkey combinations
3. Use the format: `Modifier+Modifier+Key` (e.g., `Ctrl+Shift+Y`, `Alt+F1`, `Ctrl+Alt+S`)
4. Click Save
5. Restart the application for hotkey changes to take effect

**Supported modifiers:**
- `Ctrl` or `Control`
- `Shift`
- `Alt`
- `Win` or `Windows`

**Supported keys:**
- Letter keys: A-Z
- Number keys: 0-9
- Function keys: F1-F12
- Other keys: Home, End, Delete, Insert, PageUp, PageDown, etc.

**Examples:**
- `Ctrl+Y` - Control + Y
- `Shift+Alt+T` - Shift + Alt + T
- `Ctrl+F1` - Control + F1
- `Win+S` - Windows + S

### If Hotkeys Don't Work

1. Check if another application is using the same hotkeys:
   - Close other applications
   - Try the hotkeys again

2. Run as Administrator:
   - Right-click SpellingChecker.exe
   - Select "Run as administrator"

3. Check for conflicts:
   - Some security software may block global hotkeys
   - Try disabling temporarily to test

4. Verify hotkey format:
   - Make sure the hotkey is in the correct format
   - The application will validate the format when you save settings

## Performance Tuning

### API Response Time

Factors affecting speed:
- **Model selection**: gpt-4o-mini is fastest
- **Internet connection**: Faster connection = faster response
- **Text length**: Shorter text processes faster
- **API server load**: May vary by time of day

### Cost Optimization

OpenAI charges per token (roughly 4 characters = 1 token):

For `gpt-4o-mini`:
- Input: $0.15 per 1M tokens
- Output: $0.60 per 1M tokens

Example costs:
- 100-word correction: ~$0.0001 (negligible)
- 1000 corrections per month: ~$0.10
- Daily heavy use: ~$1-5 per month

### Reducing Costs

1. Use `gpt-4o-mini` instead of `gpt-4o`
2. Select only necessary text (not entire documents)
3. Monitor usage at https://platform.openai.com/usage

## Privacy and Data

### What Data is Sent to OpenAI?

- The selected text you want corrected or translated
- System prompts (instructions to the AI)
- Model and API configuration

### What Data is NOT Sent?

- Your API key (used for authentication only)
- Other text in your documents
- Your computer information
- User behavior or usage patterns

### Data Storage

**Local Storage**:
- Settings file: `%APPDATA%\SpellingChecker\settings.json` (encrypted)

**No Cloud Storage**:
- The app does not store your text anywhere
- No history or cache of corrections/translations
- No telemetry or analytics

### OpenAI Data Policy

According to OpenAI's policy:
- API requests are not used to train models (as of March 2023)
- Data may be retained for abuse monitoring (30 days)
- Review OpenAI's current data usage policy at https://openai.com/policies/api-data-usage-policies

## Troubleshooting

### "API Key is not configured"

1. Open settings (double-click tray icon or right-click → Settings)
2. Enter your OpenAI API key
3. Click Save
4. Try again

### "API request failed: 401"

- Your API key is invalid or expired
- Get a new key from https://platform.openai.com/

### "API request failed: 429"

- You've exceeded your rate limit
- Wait a few minutes and try again
- Check your OpenAI account billing status

### "API request failed: timeout"

- Check your internet connection
- The API server may be slow
- Try increasing the timeout in the code (default: 30 seconds)

### "No text selected"

- Make sure you've selected text before pressing the hotkey
- Some applications may not support text selection copy
- Try copying the text manually first

### Application Won't Start

1. Check if .NET 9.0 is installed
2. Check Windows Event Viewer for errors
3. Try running from command line to see error messages:
   ```
   SpellingChecker.exe
   ```

## Advanced Configuration

### Custom API Endpoint

For organizations using a proxy or custom endpoint:

1. Open Settings
2. Change "API Endpoint" to your URL
3. Ensure the endpoint is OpenAI-compatible
4. Save settings

Example custom endpoints:
- Azure OpenAI: `https://YOUR-RESOURCE.openai.azure.com/`
- Local proxy: `http://localhost:8080/v1`

### Environment Variables

You can also set configuration via environment variables (takes precedence over settings file):

```
SPELLING_CHECKER_API_KEY=sk-...
SPELLING_CHECKER_API_ENDPOINT=https://api.openai.com/v1
SPELLING_CHECKER_MODEL=gpt-4o-mini
```

## Multi-User Setup

Each Windows user has their own:
- Encrypted settings file
- API key configuration
- Auto-start settings

This allows different users on the same computer to use their own API keys.

## Network and Firewall

The application requires outbound HTTPS access to:
- `api.openai.com` (or your configured endpoint)
- Port 443 (HTTPS)

If behind a corporate firewall:
1. Ensure HTTPS is allowed
2. Configure proxy settings if needed
3. Contact IT if blocked

## Backup and Restore

### Backup Settings

Settings are stored in:
```
%APPDATA%\SpellingChecker\settings.json
```

To backup:
1. Close the application
2. Copy the settings.json file
3. Store securely (file contains encrypted API key)

### Restore Settings

1. Close the application
2. Copy settings.json to `%APPDATA%\SpellingChecker\`
3. Start the application

**Note**: Encrypted settings only work for the same Windows user account.

## Uninstallation

To completely remove the application:

1. Exit the application (right-click tray icon → Exit)
2. Uninstall via Windows Settings → Apps
3. Manually delete remaining data:
   ```
   %APPDATA%\SpellingChecker
   ```
4. Remove auto-start registry key (if not removed by uninstaller):
   ```
   HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
   ```

## Support and Feedback

- GitHub Issues: https://github.com/shinepcs/SpellingChecker/issues
- Feature Requests: Create an issue with the "enhancement" label
- Bug Reports: Create an issue with the "bug" label
