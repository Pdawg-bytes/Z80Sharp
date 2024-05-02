namespace Z80Sharp.Enums
{
    public enum ProcessorMode
    {
        /// <summary>
        /// Sets the processor to debug mode. Logs all activity.
        /// </summary>          
        Debug,
        /// <summary>
        /// Sets the processor to production mode. Minimal logs generated for performance.
        /// </summary>        
        Production
    }
}