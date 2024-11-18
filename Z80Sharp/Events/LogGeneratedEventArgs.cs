using Z80Sharp.Enums;

namespace Z80Sharp.Events
{
    public class LogGeneratedEventArgs : EventArgs
    {
        public LogGeneratedEventArgs(LogSeverity logSeverity, string logData)
        {
            LogSeverity = logSeverity;
            LogData = logData;
        }

        /// <summary>
        /// The severity of the generated log.
        /// </summary>
        public LogSeverity LogSeverity { get; set; }

        /// <summary>
        /// The message data of the generated log.
        /// </summary>
        public string LogData { get; set; }
    }
}