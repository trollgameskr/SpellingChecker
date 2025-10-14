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

        // SendInput constants and structures (used by RPA tools)
        private const int INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP_SENDINPUT = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint Type;
            public INPUTUNION Union;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
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

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint EM_GETSEL = 0x00B0;
        private const uint EM_GETSELTEXT = 0x00B1;
        private const uint EM_EXGETSEL = 0x0434;

        /// <summary>
        /// Sends keyboard input using SendInput API (same as RPA tools like UiPath)
        /// </summary>
        private void SendKeyInput(ushort keyCode, bool keyUp = false)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].Type = INPUT_KEYBOARD;
            inputs[0].Union.Keyboard.wVk = keyCode;
            inputs[0].Union.Keyboard.wScan = 0;
            inputs[0].Union.Keyboard.dwFlags = keyUp ? KEYEVENTF_KEYUP_SENDINPUT : 0;
            inputs[0].Union.Keyboard.time = 0;
            inputs[0].Union.Keyboard.dwExtraInfo = IntPtr.Zero;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Simulates Ctrl+C using SendInput API (used by RPA tools like UiPath)
        /// </summary>
        private void SimulateCtrlC()
        {
            SendKeyInput(VK_CONTROL, false);  // Press Ctrl
            SendKeyInput(VK_C, false);         // Press C
            SendKeyInput(VK_C, true);          // Release C
            SendKeyInput(VK_CONTROL, true);    // Release Ctrl
        }

        /// <summary>
        /// Simulates Ctrl+V using SendInput API (used by RPA tools like UiPath)
        /// </summary>
        private void SimulateCtrlV()
        {
            SendKeyInput(VK_CONTROL, false);  // Press Ctrl
            SendKeyInput(VK_V, false);         // Press V
            SendKeyInput(VK_V, true);          // Release V
            SendKeyInput(VK_CONTROL, true);    // Release Ctrl
        }

        /// <summary>
        /// Gets the currently selected text using SendInput API (like RPA tools such as UiPath)
        /// </summary>
        public string GetSelectedText()
        {
            try
            {
                // Use clipboard method with SendInput API (same as RPA tools)
                var selectedText = GetSelectedTextViaClipboard();

                return selectedText;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets selected text using Windows SendMessage API (DEPRECATED - kept for reference)
        /// This method is no longer used as the RPA-style Ctrl+C approach is more reliable
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
        /// Gets selected text via clipboard using SendInput API (same as RPA tools like UiPath)
        /// </summary>
        private string GetSelectedTextViaClipboard()
        {
            try
            {
                // Save current clipboard content
                string? previousClipboard = null;
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

                // Clear clipboard to ensure we get fresh content
                try
                {
                    Clipboard.Clear();
                }
                catch
                {
                    // Ignore clipboard access errors
                }

                Thread.Sleep(50);

                // Simulate Ctrl+C using SendInput API (same as RPA tools)
                SimulateCtrlC();

                // Wait for clipboard to be updated
                Thread.Sleep(100);

                string selectedText = string.Empty;
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        selectedText = Clipboard.GetText();
                    }
                }
                catch
                {
                    // Ignore clipboard access errors
                }

                // Restore previous clipboard content if different
                try
                {
                    if (!string.IsNullOrEmpty(previousClipboard) && previousClipboard != selectedText)
                    {
                        Thread.Sleep(50);
                        Clipboard.SetText(previousClipboard);
                    }
                }
                catch
                {
                    // Ignore clipboard restore errors
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
        /// Replaces selected text by pasting from clipboard using SendInput API (same as RPA tools)
        /// </summary>
        public void ReplaceSelectedText(string text)
        {
            try
            {
                SetClipboard(text);
                Thread.Sleep(50);

                // Simulate Ctrl+V using SendInput API (same as RPA tools)
                SimulateCtrlV();
            }
            catch
            {
                // Fallback: just copy to clipboard
                SetClipboard(text);
            }
        }
    }
}
