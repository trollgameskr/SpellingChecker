using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Service for clipboard operations and getting selected text from other applications
    /// </summary>
    public class ClipboardService
    {
        private readonly SettingsService _settingsService;

        public ClipboardService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_MENU = 0x12; // Alt key
        private const uint KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// Gets the currently selected text using clipboard method
        /// </summary>
        public string GetSelectedText()
        {
            try
            {
                return GetSelectedTextViaClipboard();
            }
            catch (Exception ex)
            {
                // Return empty string if clipboard method fails
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to get selected text: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets selected text via clipboard method
        /// </summary>
        private string GetSelectedTextViaClipboard()
        {
            return CopySelectedTextWithRetry();
        }

        /// <summary>
        /// Waits for all modifier keys (Ctrl, Shift, Alt) to be released
        /// </summary>
        /// <param name="maxWaitMs">Maximum time to wait in milliseconds</param>
        private void WaitForModifierKeysRelease(int maxWaitMs = 500)
        {
            var stopwatch = Stopwatch.StartNew();
            
            while (stopwatch.ElapsedMilliseconds < maxWaitMs)
            {
                // Check if any modifier keys are still pressed
                // GetAsyncKeyState returns a short where the high-order bit indicates if the key is down
                bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
                bool shiftPressed = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;
                bool altPressed = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
                
                // If all modifier keys are released, we're done
                if (!ctrlPressed && !shiftPressed && !altPressed)
                {
                    return;
                }
                
                // Wait a bit before checking again
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Copies selected text using keyboard simulation with clipboard change detection
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds to wait for clipboard change</param>
        /// <returns>The copied text, or empty string if timeout or error occurred</returns>
        private string CopySelectedTextWithRetry(int timeoutMs = 1000)
        {
            try
            {
                // Wait for all hotkey modifier keys to be released before simulating Ctrl+C
                // This prevents the hotkey modifiers from interfering with the copy operation
                WaitForModifierKeysRelease();
                
                string previousClipboard = string.Empty;
                
                // Save previous clipboard content
                if (Clipboard.ContainsText())
                {
                    previousClipboard = Clipboard.GetText();
                }

                Clipboard.Clear();

                // Simulate Ctrl+C using keybd_event (proven to work in ReplaceSelectedText)
                keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
                keybd_event(VK_C, 0, 0, UIntPtr.Zero);
                keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                // Give the system time to process the input and copy to clipboard
                Thread.Sleep(10);

                if (Clipboard.ContainsText())
                {
                    string current = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(current))
                    {
                        return current;
                    }
                }
                
                // If still no content, wait and poll for clipboard changes
                var stopwatch = Stopwatch.StartNew();
                int retryInterval = 200; // Retry every 200ms
                
                while (stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    // Simulate Ctrl+C using keybd_event (proven to work in ReplaceSelectedText)
                    keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
                    keybd_event(VK_C, 0, 0, UIntPtr.Zero);
                    keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                    // Give the system time to process the input and copy to clipboard
                    Thread.Sleep(10);

                    if (Clipboard.ContainsText())
                    {
                        string current = Clipboard.GetText();
                        if (!string.IsNullOrEmpty(current))
                        {
                            return current;
                        }
                    }
                    
                    // Wait before next retry, but keep checking clipboard during wait
                    int waitEndTime = (int)(stopwatch.ElapsedMilliseconds + retryInterval);
                    while (stopwatch.ElapsedMilliseconds < waitEndTime && stopwatch.ElapsedMilliseconds < timeoutMs)
                    {
                        Thread.Sleep(50);
                        
                        if (Clipboard.ContainsText())
                        {
                            string current = Clipboard.GetText();
                            if (!string.IsNullOrEmpty(current))
                            {
                                return current;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(previousClipboard))
                {
                    Clipboard.SetText(previousClipboard);
                }
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Return empty if clipboard operations fail
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Clipboard copy failed: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets text to clipboard
        /// </summary>
        public void SetClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        /// <summary>
        /// Replaces selected text by pasting from clipboard
        /// </summary>
        public void ReplaceSelectedText(string text)
        {
            try
            {
                SetClipboard(text);
                Thread.Sleep(50);

                // Simulate Ctrl+V to paste
                keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
                keybd_event(VK_V, 0, 0, UIntPtr.Zero);
                keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
            catch (Exception ex)
            {
                // Fallback: just copy to clipboard if paste simulation fails
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to replace text: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                SetClipboard(text);
            }
        }
    }
}
