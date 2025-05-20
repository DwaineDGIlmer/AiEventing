namespace Core.Configuration
{
    /// <summary>
    /// Represents the configuration settings for the bulkhead isolation policy,  which controls the maximum number of
    /// concurrent actions and queued actions  to prevent resource exhaustion.
    /// </summary>
    /// <remarks>The <see cref="BulkheadSettings"/> class is used to configure the behavior of a bulkhead
    /// isolation policy.  This policy limits the number of concurrent operations and queued actions to ensure system
    /// stability  under high load conditions. </remarks>
    public class BulkheadSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the circuit breaker is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of concurrent operations allowed.
        /// This value determines how many tasks can be executed in parallel before additional tasks are queued or rejected.
        /// </summary>
        public int MaxParallelization { get; set; } = 10;

        /// <summary>
        /// Represents the configuration settings for the bulkhead isolation policy,
        /// which controls the maximum number of concurrent actions and queued actions
        /// to prevent resource exhaustion.
        /// </summary>
        public int MaxQueuingActions { get; set; } = 20;
    }
}
