﻿namespace Z80Sharp.Enums
{
    public enum LogSeverity
    {
        /// <summary>
        /// Processor debug information.
        /// </summary>
        Debug,

        /// <summary>
        /// General information outputted by the processor.
        /// </summary>
        Info,

        /// <summary>
        /// Logs generated by memory accesses.
        /// </summary>
        Memory,

        /// <summary>
        /// Logs generated by interrupts.
        /// </summary>
        Interrupt,

        /// <summary>
        /// Logs generated by the processor executing instructions
        /// </summary>
        Execution,

        /// <summary>
        /// Logs generated by the processor decoding instructions
        /// </summary>
        Decode,

        /// <summary>
        /// A warning generated by the processor.
        /// </summary>
        Warning,

        /// <summary>
        /// Unrecoverable events use this.
        /// </summary>
        Fatal
    }
}