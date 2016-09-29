using System;

namespace Nova.Windows.DesktopSync
{
    public struct ConsoleState
    {
        public int CursorLeft { get; set; }

        public int CursorTop { get; set; }

        public bool CursorVisible { get; set; }

        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleState(int cursorLeft, int cursorTop, ConsoleColor foregroundColor, bool cursorVisible = true)
        {
            CursorLeft = cursorLeft;
            CursorTop = cursorTop;
            ForegroundColor = foregroundColor;
            CursorVisible = cursorVisible;
        }

        /// <summary>
        /// Resets the console to a previos state.
        /// </summary>
        public void RecallState()
        {
            Console.SetCursorPosition(CursorLeft, CursorTop);
            Console.ResetColor();
            Console.CursorVisible = CursorVisible;
            Console.ForegroundColor = ForegroundColor.ToSystemColor();
        }

        /// <summary>
        /// Creates a snapshot of the console cursor properties
        /// </summary>
        /// <returns></returns>
        public static ConsoleState GetCurrentState()
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var visible = Console.CursorVisible;
            var fColor = (ConsoleColor)Console.ForegroundColor;

            return new ConsoleState(left, top, fColor, visible);
        }


    }
}