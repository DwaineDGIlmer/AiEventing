namespace Core.Contracts;

/// <summary>
/// Represents an error interface for handling errors in the application.
/// </summary>
public interface IError
{
    /// <summary>
    /// Gets the timestamp representing the current date and time in UTC, formatted as an ISO 8601 string.
    /// </summary>
    public string TimeStamp { get; }

    /// <summary>
    /// Gets or sets an error code.
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets a human-readable error message.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets a map of additional details associated with the error.
    /// </summary>
    public IList<string> ErrorDetails { get; set; }
}
