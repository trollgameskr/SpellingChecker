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

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint EM_GETSEL = 0x00B0;
        private const uint EM_GETSELTEXT = 0x00B1;
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
            catch
            {
                return string.Empty;
            }
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

                // Try to get selection using EM_GETSEL and EM_GETSELTEXT for edit controls
                IntPtr selStart = IntPtr.Zero;
                IntPtr selEnd = IntPtr.Zero;
                
                var result = SendMessage(focusedControl, EM_GETSEL, ref selStart, ref selEnd);
                
                if (selStart.ToInt32() != selEnd.ToInt32())
                {
                    // There is a selection
                    var selectionLength = selEnd.ToInt32() - selStart.ToInt32();
                    
                    if (selectionLength > 0 && selectionLength < 100000) // Sanity check
                    {
                        var buffer = new StringBuilder(selectionLength + 1);
                        SendMessage(focusedControl, EM_GETSELTEXT, IntPtr.Zero, buffer);
                        
                        if (buffer.Length > 0)
                            return buffer.ToString();
                    }
                }

                return string.Empty;
            }
            catch
            {
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
        /// Copies selected text using keyboard simulation with clipboard change detection
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds to wait for clipboard change</param>
        /// <returns>The copied text, or empty string if timeout or error occurred</returns>
        private string CopySelectedTextWithRetry(int timeoutMs = 1000)
        {
            try
            {
                string previousClipboard = string.Empty;
                
                // Save previous clipboard content
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        previousClipboard = Clipboard.GetText();
                    }
                }
                catch
                {
                    // Ignore clipboard access errors
                }

                // Clear clipboard and wait for it to take effect
                try
                {
                    Clipboard.Clear();
                    Thread.Sleep(50); // Wait for clear to complete
                }
                catch
                {
                    // Ignore clipboard access errors
                }

                // Simulate Ctrl+C using keybd_event (proven to work in ReplaceSelectedText)
                keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
                keybd_event(VK_C, 0, 0, UIntPtr.Zero);
                keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                // Give the system time to process the input and copy to clipboard
                Thread.Sleep(150);

                // Check if clipboard now has content
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        string current = Clipboard.GetText();
                        if (!string.IsNullOrEmpty(current))
                        {
                            return current;
                        }
                    }
                }
                catch
                {
                    // Ignore clipboard access errors
                }

                // If still no content, wait and poll for clipboard changes
                var stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    Thread.Sleep(50);
                    
                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            string current = Clipboard.GetText();
                            if (!string.IsNullOrEmpty(current))
                            {
                                return current;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore clipboard access errors during polling
                    }
                }

                // Timeout - restore previous clipboard if we have it
                try
                {
                    if (!string.IsNullOrEmpty(previousClipboard))
                    {
                        Clipboard.SetText(previousClipboard);
                    }
                }
                catch
                {
                    // Ignore clipboard access errors
                }
                
                return string.Empty;
            }
            catch
            {
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
            catch
            {
                // Fallback: just copy to clipboard
                SetClipboard(text);
            }
        }
    }
}
