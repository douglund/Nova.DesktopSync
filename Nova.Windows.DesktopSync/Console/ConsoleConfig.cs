using System;
using System.Collections.Generic;

namespace Nova.Windows.DesktopSync
{
    public class ConsoleConfig
    {
        static ConsoleConfig()
        {
            ColorConfig = new Dictionary<ConsoleColor, System.ConsoleColor>
            {
                { ConsoleColor.TextLabel, System.ConsoleColor.DarkGray },
                { ConsoleColor.TextValue, System.ConsoleColor.White },
                { ConsoleColor.TextHighlight, System.ConsoleColor.Yellow },
                { ConsoleColor.TextPropmpt, System.ConsoleColor.Cyan },
                { ConsoleColor.TextStatus, System.ConsoleColor.DarkGreen },
                { ConsoleColor.TextTitle, System.ConsoleColor.Blue },
                { ConsoleColor.TextTitle2, System.ConsoleColor.Magenta },
            };
        }

        public static string NewLine { get; set; } = Environment.NewLine;

        public static int IndentLevel { get; set; } = 0;

        public static IDictionary<ConsoleColor, System.ConsoleColor> ColorConfig { get; }

        public static char[] CenterPadChars { get; set; } = new []{'·'};
    }

    public enum ConsoleColor
    {
        None = 0,

        // System.ConsoleColor
        Black = System.ConsoleColor.Black,
        DarkBlue = System.ConsoleColor.DarkBlue,
        DarkGreen = System.ConsoleColor.DarkGreen,
        DarkCyan = System.ConsoleColor.DarkCyan,
        DarkRed = System.ConsoleColor.DarkRed,
        DarkMagenta = System.ConsoleColor.DarkMagenta,
        DarkYellow = System.ConsoleColor.DarkYellow,
        Gray = System.ConsoleColor.Gray,
        DarkGray = System.ConsoleColor.DarkGray,
        Blue = System.ConsoleColor.Blue,
        Green = System.ConsoleColor.Green,
        Cyan = System.ConsoleColor.Cyan,
        Red = System.ConsoleColor.Red,
        Magenta = System.ConsoleColor.Magenta,
        Yellow = System.ConsoleColor.Yellow,
        White = System.ConsoleColor.White,

        // Configurable Colors
        TextLabel,
        TextValue,
        TextHighlight,
        TextPropmpt,
        TextStatus,
        TextTitle,
        TextTitle2
    }
}