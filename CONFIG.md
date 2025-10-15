# Configuration Guide

## Getting an API Key

### OpenAI API Key

1. Visit https://platform.openai.com/
2. Sign up or log in to your account
3. Navigate to API Keys section
4. Click "Create new secret key"
5. Copy the key (you won't be able to see it again)
6. Paste it into the SpellingChecker settings

### Anthropic API Key

1. Visit https://console.anthropic.com/
2. Sign up or log in to your account
3. Navigate to API Keys section
4. Click "Create Key"
5. Copy the key
6. Paste it into the SpellingChecker settings

### Google Gemini API Key

1. Visit https://makersuite.google.com/app/apikey
2. Sign up or log in to your Google account
3. Click "Create API Key"
4. Copy the key
5. Paste it into the SpellingChecker settings

## API Key Security

Your API key is:
- Encrypted using Windows Data Protection API (DPAPI)
- Stored per-user in `%APPDATA%\SpellingChecker\settings.json`
- Never transmitted anywhere except to your selected AI provider's API
- Protected from access by other users on the same computer

## Configuration Options

### AI Provider

**Default**: `OpenAI`

Available providers:
- **OpenAI**: GPT models, most widely used and tested
- **Anthropic**: Claude models, known for helpful and harmless AI
- **Gemini**: Google's latest AI models, powerful and efficient

Select your preferred AI provider from the dropdown in settings. The API endpoint and available models will update automatically based on your selection.

**Provider-Specific API Keys**: Each provider can have its own API key. When you switch providers in the Settings window, the application will automatically load and save the API key for that specific provider. This allows you to easily switch between providers without re-entering your credentials.

### API Endpoint

The API endpoint is automatically set based on your selected provider:
- **OpenAI**: `https://api.openai.com/v1`
- **Anthropic**: `https://api.anthropic.com/v1`
- **Gemini**: `https://generativelanguage.googleapis.com/v1beta`

You can change this to:
- Use a proxy server
- Use a local API endpoint for testing
- Use a custom endpoint (for advanced users)

### AI Model

The available models depend on your selected provider:

#### OpenAI Models
**Default**: `gpt-4o-mini`
- `gpt-4o-mini`: Fast, cost-effective, good quality (recommended)
- `gpt-4o`: Slower, more expensive, highest quality
- `o1`: Advanced reasoning model for complex problems
- `o1-mini`: Faster, more affordable reasoning model

#### Anthropic Models
**Default**: `claude-sonnet-4-5`
- `claude-sonnet-4-5`: Best for coding and agents (recommended)
- `claude-3-5-sonnet-latest`: Balanced performance and quality
- `claude-3-5-haiku-latest`: Fast and efficient

#### Gemini Models
**Default**: `gemini-2.5-flash-latest`
- `gemini-2.5-pro-latest`: Complex reasoning and large dataset analysis
- `gemini-2.5-flash-latest`: Fast with impressive performance (recommended)
- `gemini-2.0-flash-exp`: Experimental fast model

**Custom Models**: You can also type any custom model name directly into the AI Model field. Custom models will be saved and appear in the dropdown for future use. This is useful for:
- Using newer models not yet in the default list
- Testing beta or experimental models
- Using provider-specific model variants

**Recommendation**: Use the default model for each provider for the best balance of speed, cost, and quality.

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

## Tone Presets (문장 톤 프리셋)

Tone presets allow you to apply different speaking styles to your corrected text. The application comes with 11 default tone presets and allows you to create custom ones.

### Default Tone Presets

1. **톤 없음** (No Tone)
   - Maintains the original tone of the text
   - Only corrects spelling and grammar

2. **근엄한 팀장님 톤** (Strict Manager Tone)
   - Authoritative and strict speaking style
   - Mixed with instructions and advice

3. **싹싹한 신입 사원 톤** (Eager New Employee Tone)
   - Bright, polite, and proactive attitude
   - Cheerful and respectful

4. **MZ세대 톤** (MZ Generation Tone)
   - Uses latest slang and internet memes
   - Casual and light-hearted

5. **심드렁한 알바생 톤** (Bored Part-timer Tone)
   - Unmotivated and indifferent style
   - Minimal response feel

6. **유난히 예의 바른 경비원 톤** (Overly Polite Guard Tone)
   - Excessively polite
   - Emphasizes procedures and formality

7. **오버하는 홈쇼핑 쇼호스트 톤** (Excited TV Shopping Host Tone)
   - Overly excited
   - Emphasizes everything as the best

8. **유행어 난발하는 예능인 톤** (Comedian with Trendy Phrases Tone)
   - Fast-paced speech with trendy words
   - Mixes humor and current slang

9. **100년 된 할머니 톤** (100-year-old Grandmother Tone)
   - Old-fashioned words and slow speech
   - Nostalgic sentences

10. **드라마 재벌 2세 톤** (Drama Chaebol Heir Tone)
    - Arrogant and luxurious atmosphere
    - Wealthy and spoiled tone

11. **외국인 한국어 학습자 톤** (Korean Learner Tone)
    - Slightly awkward grammar
    - Cute expressions mixed in

### Using Tone Presets

1. Open Settings (tray icon → Settings)
2. Find the "문장 톤 프리셋" (Tone Preset) section
3. Select your desired tone from the dropdown
4. The description will update to show what the tone does
5. Click Save to apply the tone
6. When you use spelling correction (Ctrl+Shift+Alt+Y), the selected tone will be automatically applied

### Managing Custom Tone Presets

**To Add a Custom Tone:**
1. Open Settings → Tone Presets section
2. Click "➕ 추가" (Add) button
3. Enter a name for your tone (e.g., "공식 보고서 톤")
4. Enter a description (e.g., "격식 있고 전문적인 말투, 보고서에 적합")
5. Click "확인" (OK) to save

**To Edit a Tone:**
1. Select a tone from the dropdown
2. Click "✏️ 수정" (Edit) button
3. Modify the name and/or description
4. Click "확인" (OK) to save changes

**To Delete a Tone:**
1. Select a tone from the dropdown
2. Click "🗑️ 삭제" (Delete) button
3. Confirm the deletion

**Note**: You can now edit and delete any tone preset, including the default ones. Be careful when modifying or deleting default presets as you may lose the original configurations.

### Examples of Custom Tones

Here are some ideas for custom tone presets:

- **공식 보고서 톤**: 격식 있고 전문적인 말투, 보고서 작성에 적합
- **친근한 메시지 톤**: 부드럽고 친근한 말투, 메신저 대화에 적합
- **학술 논문 톤**: 객관적이고 학문적인 표현, 논문 작성에 적합
- **블로그 포스팅 톤**: 편안하고 설명적인 말투, 블로그 글쓰기에 적합
- **이메일 공손 톤**: 정중하면서도 업무적인 말투, 공식 이메일에 적합

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

## Usage Statistics and History

### Viewing Usage Statistics

To view your API usage:
1. Open Settings (double-click tray icon or right-click → Settings)
2. Click "📊 View Usage Statistics" button
3. View your token usage, costs, and operation history

### What is Tracked

The application automatically tracks:
- **Date and time** of each operation
- **Operation type** (Correction or Translation)
- **Model used** (e.g., gpt-4o-mini)
- **Prompt tokens** (input text tokens)
- **Completion tokens** (output text tokens)
- **Total tokens** (sum of prompt + completion)
- **Estimated cost** (calculated using current OpenAI pricing)

### Filtering Statistics

You can filter statistics by period:
- **All Time**: Complete history
- **Today**: Operations from today only
- **This Week**: Current week (Monday to Sunday)
- **This Month**: Current calendar month

### Cost Calculation

Costs are calculated based on OpenAI's pricing:

**gpt-4o-mini** (default):
- Input: $0.15 per 1M tokens
- Output: $0.60 per 1M tokens

**gpt-4o**:
- Input: $5.00 per 1M tokens
- Output: $15.00 per 1M tokens

**gpt-3.5-turbo**:
- Input: $0.50 per 1M tokens
- Output: $1.50 per 1M tokens

Note: Actual costs may vary. Check https://openai.com/pricing for current rates.

### Data Storage

Usage history is stored locally at:
```
%APPDATA%\SpellingChecker\usage_history.json
```

This file:
- Contains all your usage records
- Is stored in plain JSON (not encrypted)
- Can be manually deleted if needed
- Does not contain API keys or sensitive information
- Only contains timestamps, token counts, and calculated costs

### Clearing History

To clear all usage history:
1. Open Usage Statistics window
2. Click "Clear History" button
3. Confirm the action

**Warning**: This action cannot be undone. All usage records will be permanently deleted.

### Privacy

- Usage data is stored locally only
- No data is sent to external servers (except API requests to OpenAI)
- Usage tracking can be disabled by deleting the usage_history.json file
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
