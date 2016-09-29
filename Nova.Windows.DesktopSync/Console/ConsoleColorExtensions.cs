using System;
using System.Linq;
using System.Text;
using static Nova.Windows.DesktopSync.ConsoleConfig;

namespace Nova.Windows.DesktopSync
{
    public static class ConsoleColorExtensions
    {
        public static void WriteLine(this ConsoleColor color, string message)
        {
            color.Write(message);
            color.Write(NewLine);
        }

        public static void Fill(this ConsoleColor color, char fillChar)
        {
            Fill(color, Console.CursorLeft, Console.CursorTop, fillChar);
        }

        public static void Fill(this ConsoleColor color, ConsoleState state, char fillChar)
        {
            Fill(color, state.CursorLeft, state.CursorTop, fillChar);
        }

        public static void Fill(this ConsoleColor color, int leftPosition, char fillChar)
        {
            Fill(color, leftPosition, Console.CursorTop, fillChar);
        }

        public static void Fill(this ConsoleColor color, int leftPosition, int topPosition, char fillChar)
        {
            Console.SetCursorPosition(leftPosition, topPosition);
            var line = new string(fillChar, Console.WindowWidth - Console.CursorLeft);
            color.Write(line);
        }

        public static void Write(this ConsoleColor color, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Console.ForegroundColor = color.ToSystemColor();
            Console.Write(message);
            Console.ResetColor();
        }

        public static System.ConsoleColor ToSystemColor(this ConsoleColor color)
        {
            System.ConsoleColor sysColor;
            if (!Enum.TryParse(color.ToString(), false, out sysColor))
                sysColor = ColorConfig[color];
            return sysColor;
        }

        public static void WriteLineCenter(this ConsoleColor extendColor, string message, string padLeft,string padRight)
        {
            extendColor.WriteLineCenter(message, padLeft.ToCharArray(), padRight.ToCharArray());
        }

        public static void WriteLineCenter(this ConsoleColor extendColor, string message, string padWith)
        {
            extendColor.WriteLineCenter(message, padWith.ToCharArray());
        }

        public static void WriteLineCenter(this ConsoleColor extendColor, string message)
        {
            extendColor.WriteLineCenter(message, ConsoleConfig.CenterPadChars);
        }

        public static void WriteLineCenter(this ConsoleColor extendColor, string message, params char[] padChars)
        {
            extendColor.WriteLineCenter( message, padChars, padChars);
        }

        public static void WriteLineCenter(this ConsoleColor extendColor, string message, char[] padLeft, char[] padRight, ConsoleColor paddingColor = ConsoleColor.None)
        {
            if (string.IsNullOrEmpty(message))
                return;
            
            if (paddingColor == ConsoleColor.None && !Enum.TryParse($"Dark{extendColor.ToSystemColor()}", false, out paddingColor))
                paddingColor = ConsoleColor.TextLabel;
            
            var centerMsg = message.PadCenter(Console.WindowWidth - Console.CursorLeft - 2, padLeft, padRight);
            var messageIndex = centerMsg.IndexOf(message, StringComparison.CurrentCulture);
            var output = centerMsg.Substring(0, messageIndex);
            paddingColor.Write(output);
            extendColor.Write(message);
            output = centerMsg.Substring(messageIndex + message.Length);
            paddingColor.Write(output);
            extendColor.Write(NewLine);
        }

        public static string Enc(this ConsoleColor color, string message = null)
        {
            return $"#!{color}#{message}";
        }
    }
}