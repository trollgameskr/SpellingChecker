using System;
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
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

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
        private const uint KEYEVENTF_KEYDOWN = 0x0000;

        // SendInput structures
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP_SENDINPUT = 0x0002;

        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint EM_GETSEL = 0x00B0;
        private const uint EM_GETSELTEXT = 0x00B1;
        private const uint EM_EXGETSEL = 0x0434;

        /// <summary>
        /// Sends a key press using SendInput API (more reliable than keybd_event on Windows 11)
        /// </summary>
        private void SendKeyPress(ushort keyCode, bool keyUp)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = keyCode;
            inputs[0].u.ki.wScan = 0;
            inputs[0].u.ki.dwFlags = keyUp ? KEYEVENTF_KEYUP_SENDINPUT : 0;
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Simulates Ctrl+C or Ctrl+V using SendInput API
        /// </summary>
        private void SendCtrlKey(ushort keyCode)
        {
            // Press Ctrl
            SendKeyPress(VK_CONTROL, false);
            Thread.Sleep(10);
            
            // Press the key (C or V)
            SendKeyPress(keyCode, false);
            Thread.Sleep(10);
            
            // Release the key
            SendKeyPress(keyCode, true);
            Thread.Sleep(10);
            
            // Release Ctrl
            SendKeyPress(VK_CONTROL, true);
            Thread.Sleep(10);
        }

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
            try
            {
                // Save current clipboard content to restore later
                string? previousClipboard = null;
                bool hadPreviousClipboard = false;
                if (Clipboard.ContainsText())
                {
                    previousClipboard = Clipboard.GetText();
                    hadPreviousClipboard = true;
                }

                // Simulate Ctrl+C to copy selected text using SendInput (more reliable on Windows 11)
                SendCtrlKey(VK_C);

                // Wait for clipboard to be updated
                Thread.Sleep(150);

                string selectedText = string.Empty;
                if (Clipboard.ContainsText())
                {
                    selectedText = Clipboard.GetText();
                }

                // Restore previous clipboard content if it was different
                // This preserves the user's clipboard for their other work
                if (hadPreviousClipboard && !string.IsNullOrEmpty(selectedText) && 
                    selectedText != previousClipboard)
                {
                    // Give a moment for the clipboard to be read by us first
                    Thread.Sleep(50);
                    if (!string.IsNullOrEmpty(previousClipboard))
                    {
                        Clipboard.SetText(previousClipboard);
                    }
                }

                return selectedText;
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

                // Simulate Ctrl+V to paste using SendInput (more reliable on Windows 11)
                SendCtrlKey(VK_V);
            }
            catch
            {
                // Fallback: just copy to clipboard
                SetClipboard(text);
            }
        }
    }
}
