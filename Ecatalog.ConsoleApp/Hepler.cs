using System;

namespace Ecatalog.ConsoleApp
{
    /// <summary>
    /// Hepler class with common methods
    /// </summary>
    public static class Hepler
    {
        /// <summary>
        /// Write text line to console with specified color
        /// </summary>
        /// <param name="text">Output text</param>
        /// <param name="color">Text color</param>
        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = prevColor;
        }
    }
}
