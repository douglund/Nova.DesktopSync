using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static Nova.Windows.DesktopSync.ConsoleConfig;

namespace Nova.Windows.DesktopSync
{
    public static class ConsoleExtensions
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void WriteException(this TextWriter consoleWriter, Exception ex, bool isFatal = true)
        {

            consoleWriter.WriteLine();

            var color = isFatal
                ? ConsoleColor.Red
                : ConsoleColor.Yellow;

            var title = isFatal
                ? "* ERROR: "
                : "* WARNING: ";

            color.Write(title);
            WriteExceptionTree(ex);

            if (!isFatal) return;

            ConsoleColor.Gray.WriteLineCenter("stack trace", '-');
            ConsoleColor.DarkGray.WriteLine(ex.StackTrace);
            ConsoleColor.Gray.WriteLineCenter("stack trace", '-');
            consoleWriter.WriteLine();
        }

        private static void WriteExceptionTree(Exception ex)
        {
            ConsoleColor.White.Write("→ ");
            ConsoleColor.White.WriteLine(ex.Message);

            if (ex.InnerException != null)
            {
                ConsoleConfig.IndentLevel++;
                WriteExceptionTree(ex.InnerException);
                ConsoleConfig.IndentLevel--;
            }

        }


        public static void WriteObject<T>(this TextWriter consoleWriter, T input, string label, object overrideValue = null) where T : class
        {
            consoleWriter.WriteProperty(label, overrideValue ?? input);
            if (input == null)
                return;

            ConsoleConfig.IndentLevel++;
            var properties = typeof(T).GetProperties();
            var pvQuery = from pi in properties
                select new PropertyValue(pi.Name, pi.GetValue(input));
            consoleWriter.WriteProperty(pvQuery.ToArray());
            ConsoleConfig.IndentLevel--;
        }

        public static void WriteProperty(this TextWriter consoleWriter, params PropertyValue[] args)
        {
            var requiredLabelLen = args.Max(a => a.Name.Length) + 1;
            foreach (var propertyValue in args)
            {
                ConsoleColor.TextLabel.Write(propertyValue.Name);
                var spacing = requiredLabelLen - propertyValue.Name.Length;
                Console.Write(new string(' ', spacing));
                ConsoleColor.TextLabel.Write(": ");
                ConsoleColor.TextValue.WriteLine(propertyValue.Value?.ToString() ?? "null");
            }
        }

        public static void WriteProperty(this TextWriter consoleWriter, string label, object value)
        {
            var prop = new PropertyValue(label, value);
            consoleWriter.WriteProperty(prop);
        }

        public static ConsoleKeyInfo Wait(this TextWriter consoleWriter, string message = "<PRESS ANY KEY TO CONTINUE>", ConsoleColor color = 0, bool showCursor = false)
        {
            if (Console.CursorLeft > 0)
                Console.WriteLine();
            var cursor = ConsoleState.GetCurrentState();
            if (color == 0)
                color = ConsoleColor.TextPropmpt;
            color.Write(message);
            Console.CursorVisible = showCursor;
            var r = Console.ReadKey();
            consoleWriter.ClearLine();
            cursor.RecallState();
            return r;
        }


        public static void HideWindow(this TextWriter consoleWriter)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

        }

        public static void ShowWindow(this TextWriter consoleWriter)
        {
            var handle = GetConsoleWindow();
            
            // Show
            ShowWindow(handle, SW_SHOW);

        }

        public static void ClearLine(this TextWriter consoleWriter, int? yPosition = null)
        {
            var originalCursor = ConsoleState.GetCurrentState();
            if (yPosition == null)
                yPosition = Console.CursorTop;
            ConsoleColor.TextLabel.Fill(0, yPosition.Value, ' ');
            originalCursor.RecallState();
        }

        public static void WriteLine(this TextWriter consoleWriter, params string[] messageArgs)
        {
            foreach (var message in messageArgs)
            {
                consoleWriter.Write(message);
                consoleWriter.Write(NewLine);
            }

            Console.ResetColor();
        }

        public static void Write(this TextWriter consoleWriter, params string[] encodedArgs)
        {
            foreach (var message in encodedArgs)
            {
                if (string.IsNullOrEmpty(message))
                    return;

                var msgParts = message.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var msgPart in msgParts)
                {
                    if (msgPart.StartsWith("!"))
                    {
                        System.ConsoleColor foregroundColor;
                        if (Enum.TryParse(msgPart.TrimStart('!'), true, out foregroundColor))
                        {
                            Console.ForegroundColor = foregroundColor;
                            continue;
                        }
                    }

                    consoleWriter.Write(msgPart.ToCharArray(), 0, msgPart.Length - 1);
                }
            }
        }
    }
}