using Z80Sharp.Enums;
using Z80Sharp.Events;

namespace Z80Sharp.Interfaces
{
    /// <summary>
    /// Defines the exposed logger that allows you to generate custom logs or read generated ones.
    /// </summary>
    public interface IZ80Logger
    {
        /// <summary>
        /// An event that is fired when a log is generated.
        /// </summary>
        /// <remarks>
        /// LogGeneratedEventArgs contains a <see cref="LogSeverity"/> and a <see cref="string"/>.
        /// </remarks>
        public event EventHandler<LogGeneratedEventArgs> LogGenerated;

        /// <summary>
        /// Method called to generate a log.
        /// </summary>
        /// <param name="severity">The severity of the log entry.</param>
        /// <param name="message">The data that should be logged.</param>
        /// <remarks>All parameters passed to <paramref name="message"/> are converted to a string using the object.ToString() method.</remarks>
        public void Log(LogSeverity severity, object message);
    }
}