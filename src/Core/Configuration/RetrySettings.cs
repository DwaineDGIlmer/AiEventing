namespace Core.Configuration
{
    /// <summary>
    /// Represents the configuration settings for retry logic.
    /// </summary>
    sealed public class RetrySettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether retry logic is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the initial delay in seconds between retry attempts. 
        /// </summary>
        public int Delay { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum delay allowed between retry attempts.
        /// </summary>
        public int MaxDelay { get; set; } = 10;

        /// <summary>
        /// Gets or sets the maximum random jitter to apply to the delay between retries.
        /// </summary>
        public int Jitter { get; set; } = 5;
    }
}
