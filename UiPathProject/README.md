# SpellingChecker - UiPath RPA Implementation

This is the UiPath Studio RPA implementation of the SpellingChecker application, migrated from the original .NET/C# implementation.

## Overview

This UiPath project implements AI-powered spelling correction and translation functionality using RPA (Robotic Process Automation) techniques to:
- Capture selected text from any application using Ctrl+C
- Send text to OpenAI API for correction or translation
- Display results to the user
- Allow copying to clipboard or replacing the original text

## Project Structure

```
UiPathProject/
├── project.json                       # Project configuration
├── Main.xaml                          # Main workflow - entry point
├── Workflows/
│   ├── SpellingCorrection.xaml       # Spelling correction workflow
│   ├── Translation.xaml              # Translation workflow
│   └── CallOpenAIAPI.xaml            # OpenAI API call helper
└── README.md                         # This file
```

## Prerequisites

### Required Software
- **UiPath Studio** (version 23.10 or later)
- Windows 10 or later
- Internet connection for API calls

### Required UiPath Packages
The following packages are automatically managed by UiPath Studio:
- `UiPath.System.Activities` (v23.10.0)
- `UiPath.UIAutomation.Activities` (v23.10.0)
- `UiPath.Excel.Activities` (v2.20.0)
- `UiPath.WebAPI.Activities` (v1.13.0)

### OpenAI API Key
You need an OpenAI API key to use this project. Get one from:
https://platform.openai.com/api-keys

## Installation

1. **Install UiPath Studio**
   - Download from: https://www.uipath.com/product/studio
   - Install following the official instructions

2. **Open the Project**
   - Launch UiPath Studio
   - Click **Open** → Navigate to the `UiPathProject` folder
   - Select `project.json`

3. **Install Dependencies**
   - UiPath Studio will automatically prompt to install required packages
   - Click **Install** and wait for completion

## Configuration

Before running the workflows, you need to configure your OpenAI API credentials:

1. Open `Main.xaml` in UiPath Studio
2. In the workflow properties, set the following arguments:
   - `in_APIKey`: Your OpenAI API key (e.g., "sk-...")
   - `in_APIEndpoint`: API endpoint (default: "https://api.openai.com/v1/chat/completions")

**Security Note**: Do not hardcode your API key in the workflow. Use:
- UiPath Orchestrator Assets (recommended for production)
- Config file (for development)
- Environment variables

### Using Config File

Create a file named `Config.xlsx` in the project directory with:

| Key | Value |
|-----|-------|
| APIKey | your-api-key-here |
| APIEndpoint | https://api.openai.com/v1/chat/completions |

Then modify Main.xaml to read from this file.

## Usage

### Running the Main Workflow

1. Open `Main.xaml` in UiPath Studio
2. Click **Run** (F5) or **Debug** (F6)
3. A dialog will appear with options:
   - **1. Spelling Correction**
   - **2. Translation**
   - **3. Settings** (coming soon)
   - **4. Exit**

### Spelling Correction Workflow

1. Select **1. Spelling Correction** from the menu
2. When prompted, **select text** in any application (e.g., Notepad, Word, browser)
3. Click **OK** in the instruction dialog
4. The robot will:
   - Simulate **Ctrl+C** to copy your selected text
   - Send it to OpenAI API for correction
   - Show the original and corrected text
   - Ask if you want to **Copy** or **Replace**
5. Choose your action:
   - **Copy to Clipboard**: Places corrected text in clipboard
   - **Replace Selected Text**: Automatically pastes corrected text

### Translation Workflow

1. Select **2. Translation** from the menu
2. When prompted, **select text** in any application
3. Click **OK** in the instruction dialog
4. The robot will:
   - Simulate **Ctrl+C** to copy your selected text
   - Detect language (Korean/English)
   - Send it to OpenAI API for translation
   - Show the original and translated text
   - Ask if you want to **Copy** or **Replace**
5. Choose your action as above

## How It Works

### Text Capture Mechanism

The UiPath implementation uses RPA techniques to capture selected text:

```
1. User selects text in any application
2. Robot simulates Ctrl+C keypress (SendHotkey activity)
3. Robot waits 500ms for clipboard to update (Delay activity)
4. Robot reads text from clipboard (GetClipboardText activity)
```

This approach works with **any application** that supports text selection and clipboard operations.

### API Integration

The workflows use the **HTTP Request** activity to call OpenAI API:

```
1. Prepare JSON request body with:
   - Model: gpt-4o-mini
   - System prompt (correction or translation instructions)
   - User message (selected text)
2. Send POST request to OpenAI endpoint
3. Parse JSON response to extract result
4. Display to user
```

### Text Replacement

To replace selected text:

```
1. Place corrected/translated text in clipboard (SetClipboardText)
2. Simulate Ctrl+V keypress (SendHotkey activity)
3. Text is pasted over the original selection
```

## Workflows Explained

### Main.xaml
- Entry point of the application
- Shows a menu dialog for user selection
- Routes to appropriate sub-workflow based on choice
- Manages API credentials and passes them to sub-workflows

### SpellingCorrection.xaml
- Captures selected text via Ctrl+C
- Validates text is not empty
- Prepares OpenAI prompt for spelling correction
- Calls OpenAI API (via CallOpenAIAPI.xaml)
- Parses response and extracts corrected text
- Shows results and action options
- Executes copy or replace based on user choice

### Translation.xaml
- Similar structure to SpellingCorrection.xaml
- Uses different OpenAI prompt for translation
- Auto-detects language (Korean ↔ English)
- Shows translated results
- Executes copy or replace based on user choice

### CallOpenAIAPI.xaml
- Reusable workflow for OpenAI API calls
- Accepts: API key, endpoint, request body
- Returns: API response (JSON string)
- Handles errors and timeouts
- Uses HTTP Request activity with proper headers

## Customization

### Changing AI Model

Edit the request body in SpellingCorrection.xaml or Translation.xaml:

```vb
"model":"gpt-4o-mini"  →  "model":"gpt-4"
```

### Adjusting Prompts

Modify the system message in the request body to change AI behavior:

**For Spelling Correction:**
```vb
"content":"You are a helpful assistant that corrects spelling and grammar."
```

**For Translation:**
```vb
"content":"You are a translator. If the text is in Korean, translate to English..."
```

### Adding Tone Presets

To add tone/style to corrections, modify the system prompt:

```vb
"content":"You are a helpful assistant that corrects spelling and grammar. Use a formal professional tone."
```

### Timeout Adjustment

In CallOpenAIAPI.xaml, adjust the HTTP Request timeout:

```
TimeoutMS="30000"  →  TimeoutMS="60000"  (60 seconds)
```

## Troubleshooting

### "No text selected" Error

**Cause**: No text was selected or clipboard is empty

**Solution**:
- Make sure text is highlighted before clicking OK
- Try manually copying (Ctrl+C) to verify clipboard works
- Some applications may not support clipboard operations

### "API Error: 401"

**Cause**: Invalid or missing API key

**Solution**:
- Verify your OpenAI API key is correct
- Check that the key is properly set in workflow arguments
- Ensure the key starts with "sk-"

### "API Error: 429"

**Cause**: Rate limit exceeded

**Solution**:
- Wait a few minutes before trying again
- Check your OpenAI account quota and billing
- Consider upgrading your OpenAI plan

### "API Error: timeout"

**Cause**: Request took too long

**Solution**:
- Check your internet connection
- Increase timeout in CallOpenAIAPI.xaml
- Try again with shorter text

### Robot Doesn't Capture Text

**Cause**: Timing or clipboard issues

**Solution**:
- Increase delay after Ctrl+C (currently 500ms)
- Ensure the application has focus when Ctrl+C is sent
- Try running as Administrator for restricted applications

## Comparison with Original .NET Implementation

### Advantages of UiPath Implementation

✅ **No Installation Required**: Runs from UiPath Studio, no installer needed

✅ **Visual Workflow**: Easy to understand and modify without coding

✅ **Application Agnostic**: Works with any app that supports clipboard

✅ **Easy Debugging**: Step through workflows visually in UiPath Studio

✅ **No Hotkey Registration**: Uses simulated keypresses instead of global hotkeys

### Disadvantages of UiPath Implementation

❌ **Requires UiPath Studio**: Users need UiPath installed

❌ **No System Tray**: Runs as attended automation, not background service

❌ **No Global Hotkeys**: Can't trigger with Ctrl+Shift+Alt+Y from any app

❌ **Manual Invocation**: Must run from UiPath Studio or Orchestrator

❌ **Performance**: Slightly slower due to RPA overhead

## Migration Notes

This UiPath implementation migrates the core functionality from the original .NET/C# application:

### Migrated Features
- ✅ Text capture via clipboard (Ctrl+C simulation)
- ✅ OpenAI API integration for spelling correction
- ✅ OpenAI API integration for translation
- ✅ Copy to clipboard functionality
- ✅ Replace selected text functionality
- ✅ Error handling and validation

### Not Migrated (Would Require Additional Work)
- ❌ System tray integration
- ❌ Global hotkey registration (Ctrl+Shift+Alt+Y, Ctrl+Shift+Alt+T)
- ❌ Settings window with persistent configuration
- ❌ WPF popup windows
- ❌ Usage statistics and history
- ❌ Tone presets management
- ❌ Variable name suggestion feature
- ❌ Windows startup integration
- ❌ Encrypted settings storage

### Architectural Differences

| Feature | .NET Implementation | UiPath Implementation |
|---------|-------------------|---------------------|
| Text Capture | Win32 API + Clipboard | Ctrl+C simulation |
| Hotkeys | Global hotkey registration | Manual invocation |
| UI | WPF Windows | Dialog boxes |
| Configuration | Encrypted JSON file | Workflow arguments |
| Deployment | Standalone EXE | UiPath package |
| Background Service | Yes (System tray) | No (Attended) |

## Future Enhancements

Potential improvements for the UiPath implementation:

1. **Orchestrator Integration**
   - Deploy as unattended robot
   - Centralized API key management
   - Schedule periodic health checks

2. **Enhanced UI**
   - Custom forms for settings
   - Better result display windows
   - History tracking

3. **Additional Features**
   - Variable name suggestion
   - Tone preset selection
   - Usage statistics

4. **Global Hotkey Simulation**
   - Use UiPath triggers
   - Monitor for specific key combinations
   - More seamless user experience

## Support

For issues with:
- **UiPath Studio**: Visit https://forum.uipath.com/
- **OpenAI API**: Visit https://help.openai.com/
- **This Project**: Open an issue on GitHub

## License

MIT License - Same as the original .NET implementation

## Credits

Migrated from the original SpellingChecker .NET/C# implementation.
Original project: https://github.com/trollgameskr/SpellingChecker
