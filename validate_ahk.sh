#!/bin/bash

echo "=== AutoHotkey Script Validation ==="

# 1. File exists
if [ -f "SpellingChecker.ahk" ]; then
    echo "✓ SpellingChecker.ahk exists"
else
    echo "✗ SpellingChecker.ahk not found"
    exit 1
fi

# 2. Check AutoHotkey version requirement
if grep -q "#Requires AutoHotkey v2.0" SpellingChecker.ahk; then
    echo "✓ AutoHotkey v2.0 requirement found"
else
    echo "✗ Missing AutoHotkey v2.0 requirement"
fi

# 3. Check essential functions
FUNCTIONS=(
    "InitializeApp"
    "LoadSettings"
    "SaveSettings"
    "RegisterHotkeys"
    "OnSpellingCorrectionRequested"
    "OnTranslationRequested"
    "OnVariableNameSuggestionRequested"
    "GetSelectedText"
    "CorrectSpellingAsync"
    "TranslateAsync"
    "SuggestVariableNamesAsync"
    "ShowResultPopup"
    "ShowSettingsWindow"
    "ShowUsageStatisticsWindow"
)

for func in "${FUNCTIONS[@]}"; do
    if grep -q "^${func}(" SpellingChecker.ahk; then
        echo "✓ Function $func found"
    else
        echo "✗ Function $func missing"
    fi
done

# 4. Check global variables
GLOBALS=(
    "g_ApiKey"
    "g_ApiEndpoint"
    "g_Model"
    "g_SpellingHotkey"
    "g_TranslationHotkey"
    "g_VariableNameHotkey"
    "g_TonePresets"
)

for var in "${GLOBALS[@]}"; do
    if grep -q "global $var" SpellingChecker.ahk; then
        echo "✓ Global variable $var found"
    else
        echo "✗ Global variable $var missing"
    fi
done

# 5. Check tone presets
PRESETS=(
    "default-none"
    "default-serious-manager"
    "default-eager-newbie"
    "default-mz"
)

for preset in "${PRESETS[@]}"; do
    if grep -q "$preset" SpellingChecker.ahk; then
        echo "✓ Tone preset $preset found"
    else
        echo "✗ Tone preset $preset missing"
    fi
done

echo ""
echo "=== Validation Complete ==="
