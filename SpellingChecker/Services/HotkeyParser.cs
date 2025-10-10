using System;
using System.Linq;
using System.Windows.Input;

namespace SpellingChecker.Services
{
    /// <summary>
    /// Utility class for parsing hotkey strings into modifiers and key codes
    /// </summary>
    public static class HotkeyParser
    {
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        /// <summary>
        /// Parses a hotkey string (e.g., "Ctrl+Shift+Alt+Y") into modifiers and key code
        /// </summary>
        /// <param name="hotkeyString">The hotkey string to parse</param>
        /// <param name="modifiers">Output: The modifier flags</param>
        /// <param name="key">Output: The key code</param>
        /// <returns>True if parsing was successful, false otherwise</returns>
        public static bool TryParseHotkey(string hotkeyString, out uint modifiers, out Key key)
        {
            modifiers = 0;
            key = Key.None;

            if (string.IsNullOrWhiteSpace(hotkeyString))
            {
                return false;
            }

            var parts = hotkeyString.Split('+').Select(p => p.Trim()).ToArray();
            
            if (parts.Length == 0)
            {
                return false;
            }

            // The last part is the key, everything else is modifiers
            var keyString = parts[parts.Length - 1];
            
            // Parse modifiers
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var modifier = parts[i].ToLowerInvariant();
                switch (modifier)
                {
                    case "ctrl":
                    case "control":
                        modifiers |= MOD_CONTROL;
                        break;
                    case "shift":
                        modifiers |= MOD_SHIFT;
                        break;
                    case "alt":
                        modifiers |= MOD_ALT;
                        break;
                    case "win":
                    case "windows":
                        modifiers |= MOD_WIN;
                        break;
                    default:
                        return false; // Unknown modifier
                }
            }

            // At least one modifier is required for global hotkeys
            if (modifiers == 0)
            {
                return false;
            }

            // Parse the key
            try
            {
                // Handle single character keys
                if (keyString.Length == 1)
                {
                    char c = keyString.ToUpperInvariant()[0];
                    if (c >= 'A' && c <= 'Z')
                    {
                        key = (Key)Enum.Parse(typeof(Key), c.ToString());
                        return true;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        key = (Key)Enum.Parse(typeof(Key), "D" + c);
                        return true;
                    }
                }

                // Handle named keys (e.g., F1, Home, Delete, etc.)
                if (Enum.TryParse<Key>(keyString, true, out key))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a hotkey string is in correct format and can be parsed
        /// </summary>
        /// <param name="hotkeyString">The hotkey string to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidHotkey(string hotkeyString)
        {
            return TryParseHotkey(hotkeyString, out _, out _);
        }
    }
}
