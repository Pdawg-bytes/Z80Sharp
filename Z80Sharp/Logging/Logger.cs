using System.Text;
using Z80Sharp.Enums;
using Z80Sharp.Events;
using Z80Sharp.Constants;
using Z80Sharp.Interfaces;
using System.Runtime.CompilerServices;

using SeverityData = (string Header, string HeaderColor, string TextColor);

namespace Z80Sharp.Logging
{
    public class Logger : IZ80Logger
    {
        private readonly StringBuilder _logMessageBuilder = new();
        private readonly bool _useColors;

        // Idea taken from Matcha: https://github.com/AnalogFeelings/Matcha/blob/master/Source/Matcha/Sinks/Console/ConsoleSink.cs
        private readonly Dictionary<LogSeverity, SeverityData> _severityDict = new Dictionary<LogSeverity, SeverityData>()
        {
            [LogSeverity.Debug] = ("DBG", Colors.PINK, Colors.LIGHT_PINK),
            [LogSeverity.Info] = ("INF", Colors.CYAN, Colors.LIGHT_BLUE),
            [LogSeverity.Memory] = ("MEM", Colors.GREEN, Colors.LIGHT_GREEN),
            [LogSeverity.Interrupt] = ("INT", Colors.PURPLE, Colors.LIGHT_PURPLE),
            [LogSeverity.Decode] = ("DEC", Colors.ORANGE, Colors.LIGHT_ORANGE),
            [LogSeverity.Execution] = ("EXE", Colors.ORANGE, Colors.LIGHT_ORANGE),
            [LogSeverity.Warning] = ("WRN", Colors.YELLOW, Colors.LIGHT_YELLOW),
            [LogSeverity.Fatal] = ("FTL", Colors.RED, Colors.LIGHT_RED),
        };


        public Logger(bool useColors)
        {
            _useColors = useColors;
        }

        public event EventHandler<LogGeneratedEventArgs> LogGenerated;


        public void Log(LogSeverity severity, object message)
        {
            SeverityData severityData = _severityDict[severity];

            if (_useColors)
            {
                _logMessageBuilder.Append(Colors.ANSI_RESET);
                _logMessageBuilder.Append(ColorString(Colors.WHITE, "["));

                _logMessageBuilder.Append(ColorString(Colors.DARK_GRAY, $"{DateTime.Now.ToString("d")} {DateTime.Now.ToString("HH:mm:ss.ffff")} "));
                _logMessageBuilder.Append(ColorString(severityData.HeaderColor, severityData.Header));
                _logMessageBuilder.Append(ColorString(Colors.WHITE, "]"));

                _logMessageBuilder.Append(ColorString(severityData.TextColor, $" {message.ToString()}", true));

                LogGenerated?.Invoke(this, new LogGeneratedEventArgs(severity, _logMessageBuilder.ToString()));

                _logMessageBuilder.Clear();
            }
            else
            {
                _logMessageBuilder.Append($"[{DateTime.Now.ToString("d")} {DateTime.Now.ToString("HH:mm:ss.ffff")} {severityData.Header}] {message.ToString()}");
                LogGenerated?.Invoke(this, new LogGeneratedEventArgs(severity, _logMessageBuilder.ToString()));

                _logMessageBuilder.Clear();
            }

            //LogGenerated?.Invoke(this, new LogGeneratedEventArgs(severity, (string)message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ColorString(string color, string input, bool close = false) 
        {
            if (close) return color + input + Colors.ANSI_RESET;
            else return color + input;
        }
    }
}