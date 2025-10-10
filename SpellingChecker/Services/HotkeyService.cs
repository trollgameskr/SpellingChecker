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

        public event EventHandler? SpellingCorrectionRequested;
        public event EventHandler? TranslationRequested;

        private IntPtr _windowHandle;
        private const int SPELLING_HOTKEY_ID = 1;
        private const int TRANSLATION_HOTKEY_ID = 2;

        public bool RegisterHotkeys(IntPtr windowHandle, string spellingHotkey, string translationHotkey)
        {
            _windowHandle = windowHandle;

            // Parse spelling correction hotkey
            if (!HotkeyParser.TryParseHotkey(spellingHotkey, out uint spellingModifiers, out Key spellingKey))
            {
                // Fallback to default if parsing fails
                spellingModifiers = MOD_CONTROL | MOD_SHIFT | MOD_ALT;
                spellingKey = Key.Y;
            }

            // Parse translation hotkey
            if (!HotkeyParser.TryParseHotkey(translationHotkey, out uint translationModifiers, out Key translationKey))
            {
                // Fallback to default if parsing fails
                translationModifiers = MOD_CONTROL | MOD_SHIFT | MOD_ALT;
                translationKey = Key.T;
            }

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

            return spellingRegistered && translationRegistered;
        }

        public void UnregisterHotkeys()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, SPELLING_HOTKEY_ID);
                UnregisterHotKey(_windowHandle, TRANSLATION_HOTKEY_ID);
            }
        }

        public void ProcessHotkey(int hotkeyId)
        {
            if (hotkeyId == SPELLING_HOTKEY_ID)
            {
                SpellingCorrectionRequested?.Invoke(this, EventArgs.Empty);
            }
            else if (hotkeyId == TRANSLATION_HOTKEY_ID)
            {
                TranslationRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            UnregisterHotkeys();
        }
    }
}
