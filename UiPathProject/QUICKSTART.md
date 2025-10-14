# Quick Start Guide - UiPath SpellingChecker

Get started with the UiPath RPA implementation of SpellingChecker in 5 minutes!

## Prerequisites

Before you begin, ensure you have:

1. ✅ **UiPath Studio** installed (v23.10 or later)
   - Download from: https://www.uipath.com/product/studio
   - Community Edition is sufficient

2. ✅ **OpenAI API Key**
   - Sign up at: https://platform.openai.com/
   - Create an API key from the dashboard
   - Key format: `sk-...`

3. ✅ **Windows 10 or later**

4. ✅ **Internet connection**

## Installation (2 minutes)

### Step 1: Clone or Download the Project
```bash
git clone https://github.com/trollgameskr/SpellingChecker.git
cd SpellingChecker/UiPathProject
```

Or download the ZIP and extract it.

### Step 2: Open in UiPath Studio
1. Launch **UiPath Studio**
2. Click **Open** (or press Ctrl+O)
3. Navigate to the `UiPathProject` folder
4. Select `project.json`
5. Click **Open**

### Step 3: Install Dependencies
UiPath Studio will prompt you to install packages:
1. Click **Restore**
2. Wait for packages to download (~1 minute)
3. You'll see "All dependencies installed" message

✅ Installation complete!

## Configuration (1 minute)

You need to configure your OpenAI API key. Choose one method:

### Method 1: Edit Main.xaml (Simplest for testing)

1. In UiPath Studio, open `Main.xaml`
2. Click on the workflow canvas (outside any activity)
3. In **Properties** panel on the right, find **Arguments** section
4. Set the values:
   - `in_APIKey`: `"sk-your-actual-api-key-here"` (with quotes!)
   - `in_APIEndpoint`: `"https://api.openai.com/v1/chat/completions"` (or leave default)

**⚠️ Important**: Include the quotes in the value!

### Method 2: Use Config File (Recommended for repeated use)

1. Create `Config.xlsx` in the UiPathProject folder:

| Key | Value |
|-----|-------|
| APIKey | sk-your-api-key-here |
| APIEndpoint | https://api.openai.com/v1/chat/completions |

2. Modify `Main.xaml` to read from this file (requires Excel activities)

### Method 3: Orchestrator Assets (For production)

If deploying to UiPath Orchestrator:
1. Create a Credential asset named `OpenAI_APIKey`
2. Modify `Main.xaml` to retrieve the asset
3. More secure for production environments

## First Run (2 minutes)

### Step 1: Run the Main Workflow

1. Make sure `Main.xaml` is open
2. Click **Run** (or press F5)
3. Wait for the workflow to start

### Step 2: Select an Action

A dialog will appear with options:
```
┌─────────────────────────────────────┐
│  SpellingChecker - Select Action    │
├─────────────────────────────────────┤
│  1. Spelling Correction             │
│  2. Translation                     │
│  3. Settings                        │
│  4. Exit                            │
└─────────────────────────────────────┘
```

Let's try **Spelling Correction**:
1. Click **1. Spelling Correction**
2. Click **OK**

### Step 3: Prepare Text to Correct

1. Open **Notepad** (or any text editor)
2. Type some text with errors:
   ```
   Ths is a tst of the speling checker. It should corect my mistakes.
   ```
3. **Select all the text** (Ctrl+A)

### Step 4: Continue the Workflow

Back in the workflow:
1. A dialog says: "Please select the text you want to correct..."
2. Make sure your text is **still selected** in Notepad
3. Click **OK** in the dialog

### Step 5: Watch the Magic! ✨

The robot will:
1. Simulate Ctrl+C to copy your text
2. Send it to OpenAI API
3. Show you the results:

```
┌─────────────────────────────────────┐
│  Results                            │
├─────────────────────────────────────┤
│  Original:                          │
│  Ths is a tst of the speling       │
│  checker. It should corect my       │
│  mistakes.                          │
│                                     │
│  Corrected:                         │
│  This is a test of the spelling    │
│  checker. It should correct my      │
│  mistakes.                          │
└─────────────────────────────────────┘
```

### Step 6: Choose an Action

Another dialog asks what to do:
```
┌─────────────────────────────────────┐
│  What would you like to do?         │
├─────────────────────────────────────┤
│  Copy to Clipboard                  │
│  Replace Selected Text              │
└─────────────────────────────────────┘
```

Choose:
- **Copy to Clipboard**: Corrected text goes to clipboard (paste with Ctrl+V)
- **Replace Selected Text**: Robot automatically replaces the text in Notepad

🎉 **Congratulations!** You've successfully used SpellingChecker!

## Try Translation

Now let's try translation:

1. Run `Main.xaml` again (F5)
2. Select **2. Translation**
3. In Notepad, type Korean text:
   ```
   안녕하세요! 이것은 번역 테스트입니다.
   ```
4. Select the text
5. Click OK in the dialog
6. Wait for translation
7. See the result:
   ```
   Original: 안녕하세요! 이것은 번역 테스트입니다.
   Translation: Hello! This is a translation test.
   ```
8. Choose Copy or Replace

Or try English to Korean:
```
Hello! This is a translation test.
→ 안녕하세요! 이것은 번역 테스트입니다.
```

## Tips & Tricks

### 💡 Tip 1: Always Select Text First
Make sure text is selected (highlighted) before clicking OK in the instruction dialog.

### 💡 Tip 2: Works in Any Application
Try it in:
- Microsoft Word
- Excel
- Web browsers (Chrome, Edge, Firefox)
- Email clients (Outlook)
- Code editors (VS Code, Notepad++)

### 💡 Tip 3: Debug Mode
If something goes wrong:
1. Use **Debug** (F6) instead of Run
2. Step through activities with F11
3. Check variable values in the **Locals** panel

### 💡 Tip 4: Check Execution Log
View the **Output** panel at the bottom of Studio to see:
- Log messages
- API calls
- Errors

### 💡 Tip 5: Adjust Delays
If text capture fails:
1. Open `SpellingCorrection.xaml`
2. Find the **Delay** activity after "Send Ctrl+C"
3. Change duration from `00:00:00.500` to `00:00:01` (1 second)
4. Save and try again

## Common Issues & Solutions

### ❌ "No text selected"

**Problem**: Clipboard was empty when robot tried to read it

**Solutions**:
- Make sure text is **highlighted** before clicking OK
- Try manually copying (Ctrl+C) to verify clipboard works
- Increase delay after Ctrl+C in the workflow

### ❌ "API Error: 401"

**Problem**: Invalid API key

**Solutions**:
- Check your API key is correct
- Make sure it starts with `sk-`
- Verify you've entered it in the workflow arguments with quotes: `"sk-..."`
- Check the key hasn't expired at https://platform.openai.com/

### ❌ "API Error: 429"

**Problem**: Too many requests (rate limit)

**Solutions**:
- Wait 1-2 minutes and try again
- Check your OpenAI account quota
- Upgrade your OpenAI plan if needed

### ❌ "Dependencies not installed"

**Problem**: Required packages missing

**Solutions**:
- Click **Manage Packages** (Ctrl+P)
- Click **Restore** button
- Wait for all packages to download
- Restart UiPath Studio if needed

### ❌ Robot doesn't capture text

**Problem**: Timing or focus issues

**Solutions**:
- Click on the window with selected text before clicking OK
- Increase delay after Ctrl+C (see Tip 5 above)
- Run as Administrator (right-click UiPath Studio → Run as administrator)

## Next Steps

Now that you've got the basics:

### 🚀 Customize It
- Edit the AI prompts in the workflows
- Change the model from `gpt-4o-mini` to `gpt-4` (costs more)
- Add your own workflows

### 📦 Deploy It
- Publish to UiPath Orchestrator
- Schedule automatic runs
- Share with your team

### 🎨 Extend It
- Add tone presets (formal, casual, etc.)
- Add variable name suggestion workflow
- Track usage statistics

### 📖 Learn More
- Read the full [README.md](README.md)
- Study the [Migration Guide](../UIPATH_MIGRATION_GUIDE.md)
- Explore UiPath Academy: https://academy.uipath.com/

## Getting Help

- **UiPath Forum**: https://forum.uipath.com/
- **OpenAI Help**: https://help.openai.com/
- **GitHub Issues**: Report bugs or request features

## Summary

You've learned:
- ✅ How to open a UiPath project
- ✅ How to configure API keys
- ✅ How to run workflows
- ✅ How to use spelling correction
- ✅ How to use translation
- ✅ How to troubleshoot common issues

**Total time**: ~5 minutes

Happy automating! 🤖✨
