using Core.Constants;

namespace Core.Configuration
{
    /// <summary>
    /// Represents the configuration settings for a circuit breaker mechanism.
    /// </summary>
    /// <remarks>A circuit breaker is used to prevent repeated execution of operations that are likely to
    /// fail, allowing the system to recover and avoid cascading failures. These settings control the behavior of the
    /// circuit breaker, such as whether it is enabled, how long the circuit remains open, and the failure threshold
    /// that triggers the circuit to open.</remarks>
    public class CircuitBreakerSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the circuit breaker is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the duration of break when the circuit is open.
        /// </summary>
        /// <remarks>This is in seconds.</remarks>
        public int DurationOfBreak { get; set; } = Defaults.DurationOfBreak;

        /// <summary>
        /// Gets or sets the failure threshold (e.g., 0.5 for 50%).
        /// </summary>
        public int FailureThreshold { get; set; } = 5;
    }
}
