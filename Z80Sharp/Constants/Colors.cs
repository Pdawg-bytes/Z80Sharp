using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Constants
{
    public static class Colors
    {
        /*
         * Colors taken from Matcha: https://github.com/AnalogFeelings/Matcha/blob/master/Source/Matcha/Sinks/Console/ColorConstants.cs
         */

        /// <summary>
        /// The ANSI escape sequence for white.
        /// </summary>
        public const string WHITE = "\u001b[38;2;255;255;255m";

        /// <summary>
        /// The ANSI escape sequence for light gray.
        /// </summary>
        public const string LIGHT_GRAY = "\u001b[38;2;211;211;211m";

        /// <summary>
        /// The ANSI escape sequence for cyan.
        /// </summary>
        public const string CYAN = "\u001b[38;2;0;255;255m";

        /// <summary>
        /// The ANSI escape sequence for light blue.
        /// </summary>
        public const string LIGHT_BLUE = "\u001b[38;2;173;216;230m";

        /// <summary>
        /// The ANSI escape sequence for green.
        /// </summary>
        public const string GREEN = "\u001b[38;2;0;255;0m";

        /// <summary>
        /// The ANSI escape sequence for light green.
        /// </summary>
        public const string LIGHT_GREEN = "\u001b[38;2;144;238;144m";

        /// <summary>
        /// The ANSI escape sequence for yellow.
        /// </summary>
        public const string YELLOW = "\u001b[38;2;255;255;0m";

        /// <summary>
        /// The ANSI escape sequence for light yellow.
        /// </summary>
        public const string LIGHT_YELLOW = "\u001b[38;2;238;232;170m";

        /// <summary>
        /// The ANSI escape sequence for red.
        /// </summary>
        public const string RED = "\u001b[38;2;255;0;0m";

        /// <summary>
        /// The ANSI escape sequence for light red.
        /// </summary>
        public const string LIGHT_RED = "\u001b[38;2;205;92;92m";

        /// <summary>
        /// The ANSI escape sequence for orange.
        /// </summary>
        public const string ORANGE = "\u001b[38;2;255;128;0m";

        /// <summary>
        /// The ANSI escape sequence for brown.
        /// </summary>
        public const string BROWN = "\u001b[38;2;255;162;69m";

        /// <summary>
        /// The ANSI escape sequence for brown.
        /// </summary>
        public const string PINK = "\u001b[38;2;209;6;145m";

        /// <summary>
        /// The ANSI escape sequence for light pink.
        /// </summary>
        public const string LIGHT_PINK = "\u001b[38;2;201;91;166m";

        /// <summary>
        /// The ANSI escape sequence to reset all styles.
        /// </summary>
        public const string ANSI_RESET = "\u001b[0m";

        /// <summary>
        /// The ANSI escape sequence for bold text.
        /// </summary>
        public const string ANSI_BOLD = "\u001b[1m";
        /// <summary>
        /// The ANSI escape sequence for resetting the bold property.
        /// </summary>
        public const string ANSI_BOLDRESET = "\u001b[22m";

        /// <summary>
        /// The ANSI escape sequence for italic text.
        /// </summary>
        public const string ANSI_ITALIC = "\u001b[3m";
        /// <summary>
        /// The ANSI escape sequence for resetting the italic property.
        /// </summary>
        public const string ANSI_ITALICRESET = "\u001b[23m";
    }
}
