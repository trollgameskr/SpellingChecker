using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for registering and handling global hotkeys
    /// </summary>
    public class HotkeyService : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifiers
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        public event EventHandler? CommonQuestionRequested;
        public event EventHandler? SpellingCorrectionRequested;
        public event EventHandler? TranslationRequested;
        public event EventHandler? VariableNameSuggestionRequested;

        private IntPtr _windowHandle;
        private const int COMMON_QUESTION_HOTKEY_ID = 1;
        private const int SPELLING_HOTKEY_ID = 2;
        private const int TRANSLATION_HOTKEY_ID = 3;
        private const int VARIABLE_NAME_SUGGESTION_HOTKEY_ID = 4;

        public bool RegisterHotkeys(IntPtr windowHandle, string commonQuestionHotkey, string spellingHotkey, string translationHotkey, string variableNameSuggestionHotkey)
        {
            _windowHandle = windowHandle;

            // Parse common question hotkey
            if (!HotkeyParser.TryParseHotkey(commonQuestionHotkey, out uint commonQuestionModifiers, out Key commonQuestionKey))
            {
                // Fallback to default if parsing fails
                commonQuestionModifiers = MOD_ALT;
                commonQuestionKey = Key.D1;
            }

            // Parse spelling correction hotkey
            if (!HotkeyParser.TryParseHotkey(spellingHotkey, out uint spellingModifiers, out Key spellingKey))
            {
                // Fallback to default if parsing fails
                spellingModifiers = MOD_ALT;
                spellingKey = Key.D2;
            }

            // Parse translation hotkey
            if (!HotkeyParser.TryParseHotkey(translationHotkey, out uint translationModifiers, out Key translationKey))
            {
                // Fallback to default if parsing fails
                translationModifiers = MOD_ALT;
                translationKey = Key.D3;
            }

            // Parse variable name suggestion hotkey
            if (!HotkeyParser.TryParseHotkey(variableNameSuggestionHotkey, out uint variableNameModifiers, out Key variableNameKey))
            {
                // Fallback to default if parsing fails
                variableNameModifiers = MOD_ALT;
                variableNameKey = Key.D4;
            }

            // Register common question hotkey
            var commonQuestionRegistered = RegisterHotKey(
                _windowHandle,
                COMMON_QUESTION_HOTKEY_ID,
                commonQuestionModifiers,
                (uint)KeyInterop.VirtualKeyFromKey(commonQuestionKey)
            );

            // Register spelling correction hotkey
            var spellingRegistered = RegisterHotKey(
                _windowHandle,
                SPELLING_HOTKEY_ID,
                spellingModifiers,
                (uint)KeyInterop.VirtualKeyFromKey(spellingKey)
            );

            // Register translation hotkey
            var translationRegistered = RegisterHotKey(
                _windowHandle,
                TRANSLATION_HOTKEY_ID,
                translationModifiers,
                (uint)KeyInterop.VirtualKeyFromKey(translationKey)
            );

            // Register variable name suggestion hotkey
            var variableNameRegistered = RegisterHotKey(
                _windowHandle,
                VARIABLE_NAME_SUGGESTION_HOTKEY_ID,
                variableNameModifiers,
                (uint)KeyInterop.VirtualKeyFromKey(variableNameKey)
            );

            return commonQuestionRegistered && spellingRegistered && translationRegistered && variableNameRegistered;
        }

        public void UnregisterHotkeys()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, COMMON_QUESTION_HOTKEY_ID);
                UnregisterHotKey(_windowHandle, SPELLING_HOTKEY_ID);
                UnregisterHotKey(_windowHandle, TRANSLATION_HOTKEY_ID);
                UnregisterHotKey(_windowHandle, VARIABLE_NAME_SUGGESTION_HOTKEY_ID);
            }
        }

        public void ProcessHotkey(int hotkeyId)
        {
            if (hotkeyId == COMMON_QUESTION_HOTKEY_ID)
            {
                CommonQuestionRequested?.Invoke(this, EventArgs.Empty);
            }
            else if (hotkeyId == SPELLING_HOTKEY_ID)
            {
                SpellingCorrectionRequested?.Invoke(this, EventArgs.Empty);
            }
            else if (hotkeyId == TRANSLATION_HOTKEY_ID)
            {
                TranslationRequested?.Invoke(this, EventArgs.Empty);
            }
            else if (hotkeyId == VARIABLE_NAME_SUGGESTION_HOTKEY_ID)
            {
                VariableNameSuggestionRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            UnregisterHotkeys();
        }
    }
}
