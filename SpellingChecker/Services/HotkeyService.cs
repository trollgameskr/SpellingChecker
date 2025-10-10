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

        public bool RegisterHotkeys(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;

            // Ctrl+Shift+Alt+Y for spelling correction
            var spellingRegistered = RegisterHotKey(
                _windowHandle,
                SPELLING_HOTKEY_ID,
                MOD_CONTROL | MOD_SHIFT | MOD_ALT,
                (uint)KeyInterop.VirtualKeyFromKey(Key.Y)
            );

            // Ctrl+Shift+Alt+T for translation
            var translationRegistered = RegisterHotKey(
                _windowHandle,
                TRANSLATION_HOTKEY_ID,
                MOD_CONTROL | MOD_SHIFT | MOD_ALT,
                (uint)KeyInterop.VirtualKeyFromKey(Key.T)
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
