using Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Core.Configuration
{
    /// <summary>
    /// Represents a configuration for resilient HTTP policies, including retry, circuit breaker, and bulkhead settings.
    /// </summary>
    /// <remarks>This class is used to define the settings for HTTP client resilience strategies, such as
    /// retry policies,  circuit breaker policies, and bulkhead isolation. It allows customization of various parameters
    /// to  control the behavior of HTTP requests in scenarios where reliability and fault tolerance are
    /// required.</remarks>
    sealed public class ResilientHttpPolicy
    {
        /// <summary>
        /// Gets or sets the name of the HTTP client to be used for making requests.
        /// </summary>

        [Required(ErrorMessage = "HttpClientName is required.")]
        public string HttpClientName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the policy associated with the current operation.
        /// </summary>
        public string PolicyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the order in which policies are applied.
        /// </summary>
        [Range(0, 3, ErrorMessage = "PolicyOrder must be between 0 and 3.")]
        public int PolicyOrder { get; set; } = 0;

        /// <summary>
        /// Gets or sets the timeout duration, in seconds, for HTTP requests.
        /// </summary>
        public int HttpTimeout { get; set; } = Defaults.HttpTimeout;

        /// <summary>
        /// Gets or sets a value indicating whether standard resilience mechanisms are enabled.
        /// </summary>
        public bool UseStandardResilience { get; set; } = true;

        /// <summary>
        /// Gets or sets the retry policy settings for operations that may fail.
        /// </summary>
        public RetrySettings RetryPolicy { get; set; } = new RetrySettings();

        /// <summary>
        /// Gets or sets the circuit breaker policy settings used to control the behavior of transient fault handling.
        /// </summary>
        /// <remarks>The circuit breaker policy determines how the system handles repeated failures, such
        /// as temporarily halting operations  after a specified number of consecutive errors. Adjust these settings to
        /// fine-tune fault tolerance and recovery behavior.</remarks>
        public CircuitBreakerSettings CircuitBreakerPolicy { get; set; } = new CircuitBreakerSettings();

        /// <summary>
        /// Gets or sets the bulkhead policy settings used to control the maximum concurrency and queue size for
        /// operations.
        /// </summary>
        public BulkheadSettings BulkheadPolicy { get; set; } = new BulkheadSettings();
    }
}
