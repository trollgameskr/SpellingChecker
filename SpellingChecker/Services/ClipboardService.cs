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
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_MENU = 0x12; // Alt key
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint EM_GETSEL = 0x00B0;
        private const uint EM_EXGETSEL = 0x0434;

        /// <summary>
        /// Gets the currently selected text using SendMessage API
        /// </summary>
        public string GetSelectedText()
        {
            try
            {
                // First, try using SendMessage to get selected text directly
                var selectedText = GetSelectedTextViaSendMessage();
                
                // If SendMessage fails, fallback to clipboard method
                if (string.IsNullOrEmpty(selectedText))
                {
                    selectedText = GetSelectedTextViaClipboard();
                }

                return selectedText;
            }
            catch (Exception ex)
            {
                // Return empty string if all methods fail
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"Failed to get selected text: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                return string.Empty;
            }
        }

        /// <summary>
        /// Adjusts the index from EM_GETSEL to account for \r characters in line breaks.
        /// Some controls count line breaks as single characters in EM_GETSEL (\n), but store them as \r\n.
        /// This function maps logical indices to actual string indices.
        /// </summary>
        /// <param name="text">The text retrieved from WM_GETTEXT</param>
        /// <param name="logicalIndex">The index from EM_GETSEL (may count \r\n as single position)</param>
        /// <returns>The adjusted index for the actual string with \r\n</returns>
        private int GetAdjustedIndex(string text, int logicalIndex)
        {
            // EM_GETSEL may count positions as: \n(0) a(1) b(2) \n(3) c(4)
            // But actual text is: \r\n a b \r\n c (with \r characters)
            // So logical position 4 maps to actual position 6
            
            int actualIndex = 0;
            int logicalPos = 0;
            
            for (int i = 0; i < text.Length && logicalPos < logicalIndex; i++)
            {
                if (text[i] == '\r')
                {
                    // Skip \r characters - they don't count in logical position
                    actualIndex++;
                }
                else
                {
                    // This character counts in logical position
                    logicalPos++;
                    actualIndex++;
                }
            }
            
            return actualIndex;
        }

        /// <summary>
        /// Gets selected text using Windows SendMessage API
        /// </summary>
        private string GetSelectedTextViaSendMessage()
        {
            try
            {
                var foregroundWindow = GetForegroundWindow();
                if (foregroundWindow == IntPtr.Zero)
                    return string.Empty;

                // Get the focused control in the foreground window
                var currentThreadId = GetCurrentThreadId();
                var foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
                
                IntPtr focusedControl = IntPtr.Zero;
                
                if (foregroundThreadId != currentThreadId)
                {
                    // Attach to the foreground thread to get its focused control
                    AttachThreadInput(currentThreadId, foregroundThreadId, true);
                    focusedControl = GetFocus();
                    AttachThreadInput(currentThreadId, foregroundThreadId, false);
                }
                else
                {
                    focusedControl = GetFocus();
                }

                if (focusedControl == IntPtr.Zero)
                    focusedControl = foregroundWindow;

                // Try to get selection using EM_GETSEL and WM_GETTEXT for edit controls
                IntPtr selStart = IntPtr.Zero;
                IntPtr selEnd = IntPtr.Zero;
                
                var result = SendMessage(focusedControl, EM_GETSEL, ref selStart, ref selEnd);
                
                if (selStart.ToInt32() != selEnd.ToInt32())
                {
                    // There is a selection
                    var selectionLength = selEnd.ToInt32() - selStart.ToInt32();
                    
                    if (selectionLength > 0 && selectionLength < 100000) // Sanity check
                    {
                        // Get the entire text first
                        var textLength = (int)SendMessage(focusedControl, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
                        
                        if (textLength > 0 && selEnd.ToInt32() <= textLength)
                        {
                            var buffer = new StringBuilder(textLength + 1);
                            SendMessage(focusedControl, WM_GETTEXT, new IntPtr(buffer.Capacity), buffer);
                            
                            if (buffer.Length > 0)
                            {
                                // Extract the selected portion
                                var startIndex = selStart.ToInt32();
                                var endIndex = selEnd.ToInt32();
                                
                                // EM_GETSEL returns indices that count \r\n as 2 characters
                                // But in the actual string, we need to map these indices correctly
                                // Count the number of \r characters before startIndex and endIndex
                                // to adjust for the difference between control indices and string indices
                                var text = buffer.ToString();
                                
                                // Adjust indices by counting line breaks before each position
                                var adjustedStart = GetAdjustedIndex(text, startIndex);
                                var adjustedEnd = GetAdjustedIndex(text, endIndex);
                                
                                if (adjustedStart >= 0 && adjustedEnd <= text.Length && adjustedStart < adjustedEnd)
                                {
                                    return text.Substring(adjustedStart, adjustedEnd - adjustedStart);
                                }
                            }
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // Return empty if SendMessage API fails (may not be an edit control)
                // Could add logging here in future: System.Diagnostics.Debug.WriteLine($"SendMessage failed: {ex.Message}");
                _ = ex; // Suppress warning about unused variable
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets selected text via clipboard as fallback method
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

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, ref IntPtr wParam, ref IntPtr lParam);

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
