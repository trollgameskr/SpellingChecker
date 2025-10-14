using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

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
        /// Gets the currently selected text using multiple methods
        /// </summary>
        public string GetSelectedText()
        {
            try
            {
                // First, try using SendMessage to get selected text directly
                var selectedText = GetSelectedTextViaSendMessage();
                
                // If SendMessage fails, try UI Automation (works on Windows 11)
                if (string.IsNullOrEmpty(selectedText))
                {
                    selectedText = GetSelectedTextViaUIAutomation();
                }
                
                // If UI Automation fails, fallback to clipboard method
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
        /// Gets selected text using UI Automation (works reliably on Windows 11)
        /// </summary>
        private string GetSelectedTextViaUIAutomation()
        {
            try
            {
                // Get the focused element using UI Automation
                AutomationElement? focusedElement = AutomationElement.FocusedElement;
                
                if (focusedElement == null)
                    return string.Empty;

                // Try to get the text using TextPattern (most reliable for text controls)
                object? patternObject;
                if (focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObject))
                {
                    TextPattern? textPattern = patternObject as TextPattern;
                    if (textPattern != null)
                    {
                        var selections = textPattern.GetSelection();
                        if (selections != null && selections.Length > 0)
                        {
                            // Get the text from the first selection
                            string selectedText = selections[0].GetText(-1);
                            if (!string.IsNullOrEmpty(selectedText))
                                return selectedText;
                        }
                    }
                }

                // Alternative: Try ValuePattern for simple text controls
                if (focusedElement.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject))
                {
                    ValuePattern? valuePattern = patternObject as ValuePattern;
                    if (valuePattern != null)
                    {
                        return valuePattern.Current.Value ?? string.Empty;
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
        /// Note: This method reads from clipboard without copying, so users need to manually copy (Ctrl+C) first.
        /// </summary>
        private string GetSelectedTextViaClipboard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    return Clipboard.GetText();
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
                // Note: On Windows 11, this may not work in some applications due to security restrictions
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
