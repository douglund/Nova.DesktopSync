using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.Windows.DesktopSync
{
    public static class StringExtensions
    {

        public static string RemoveAll(this string input, params char[] replace)
        {

            var output = new string(input.Except(replace).ToArray());
            return output;

        }

        /// <summary>
        /// Pads the string equally from the left and right with spaces to center text horizontally.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="resultLength">Length of the result.</param>
        public static void PadCenter(this string input, int resultLength)
        {
            PadCenter(input, resultLength, ' ');
        }

        /// <summary>
        /// Pads the string equally from the left and right with the padding char(s) to center text horizontally.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="resultLength">Length of the result.</param>
        /// <param name="padChars">The pad chars.</param>
        public static void PadCenter(this string input, int resultLength, params char[] padChars)
        {
            PadCenter(input, resultLength, padChars, padChars);
        }

        /// <summary>
        /// Pads the string equally from the left and right hand sides to center text horizontally.
        /// left hand padding characters may differ from characters padding the right hand side
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="resultLength">Length of the result.</param>
        /// <param name="padLeft">The pad left.</param>
        /// <param name="padRight">The pad right.</param>
        /// <returns></returns>
        public static string PadCenter(this string input, int resultLength, char[] padLeft, char[] padRight)
        {
            return input?.PadCenter(resultLength, new string(padLeft), new string( padRight));
        }

        /// <summary>
        /// Pads the string equally from the left and right hand sides to center text horizontally.
        /// left hand padding characters may differ from characters padding the right hand side
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="resultLength">Length of the result.</param>
        /// <param name="padLeft">The pad left.</param>
        /// <param name="padRight">The pad right.</param>
        /// <returns></returns>
        public static string PadCenter(this string input, int resultLength, string padLeft, string padRight)
        {
            if (resultLength <= input?.Length)
                return input.Substring(0, resultLength);

            var paddingLength = (resultLength - (input?.Length ?? 0)) / 2;

            var resultBuilder = new StringBuilder(padLeft.Repeat(paddingLength));
            resultBuilder.Append(input);
            resultBuilder.Append(padRight.Repeat(resultLength - resultBuilder.Length));
            return resultBuilder.ToString();
        }

        /// <summary>
        /// Repeats the string until the result length is reached.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="resultLength">Length of the result.</param>
        /// <returns></returns>
        public static string Repeat(this string input, int resultLength)
        {
            if (input.Length >= resultLength)
                return input;

            var resultIndex = 0;
            var charUbound = input.Length - 1;
            var resultBuilder = new StringBuilder();

            while (resultBuilder.Length < resultLength)
            {
                resultBuilder.Append(input[resultIndex]);
                resultIndex = (resultIndex == charUbound)
                    ? 0
                    : resultIndex + 1;
            }

            return resultBuilder.ToString();
        }
    }
}