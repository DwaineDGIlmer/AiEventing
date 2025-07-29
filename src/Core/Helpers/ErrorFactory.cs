using Core.Contracts;
using Core.Models;

namespace Core.Helpers;

/// <summary>
/// Factory class for creating consistent error objects across different application layers.
/// </summary>
public static class ErrorFactory
{
    /// <summary>
    /// Creates a workflow-specific error with consistent formatting.
    /// </summary>
    /// <param name="workflowName">The name of the workflow where the error occurred.</param>
    /// <param name="errorCode">The specific error code for this workflow error.</param>
    /// <param name="errorMessage">The error message describing what went wrong.</param>
    /// <param name="errorDetails">Optional additional details about the error.</param>
    /// <returns>An <see cref="IError"/> instance with workflow-specific formatting.</returns>
    public static Error CreateWorkflowError(
        string workflowName,
        string errorCode,
        string errorMessage,
        IList<string>? errorDetails = null)
    {
        return new Error
        {
            ErrorCode = $"WF_{errorCode}",
            ErrorMessage = $"[Workflow:{workflowName}] {errorMessage}",
            ErrorDetails = errorDetails ?? []
        };
    }

    /// <summary>
    /// Creates a service-specific error with consistent formatting.
    /// </summary>
    /// <param name="serviceName">The name of the service where the error occurred.</param>
    /// <param name="errorCode">The specific error code for this service error.</param>
    /// <param name="errorMessage">The error message describing what went wrong.</param>
    /// <param name="errorDetails">Optional additional details about the error.</param>
    /// <returns>An <see cref="IError"/> instance with service-specific formatting.</returns>
    public static Error CreateServiceError(
        string serviceName,
        string errorCode,
        string errorMessage,
        IList<string>? errorDetails = null)
    {
        return new Error
        {
            ErrorCode = $"SVC_{errorCode}",
            ErrorMessage = $"[Service:{serviceName}] {errorMessage}",
            ErrorDetails = errorDetails ?? []
        };
    }

    /// <summary>
    /// Creates a general application error with consistent formatting.
    /// </summary>
    /// <param name="componentName">The name of the component where the error occurred.</param>
    /// <param name="errorCode">The specific error code for this application error.</param>
    /// <param name="errorMessage">The error message describing what went wrong.</param>
    /// <param name="errorDetails">Optional additional details about the error.</param>
    /// <returns>An <see cref="IError"/> instance with application-specific formatting.</returns>
    public static IError CreateApplicationError(
        string componentName,
        string errorCode,
        string errorMessage,
        IList<string>? errorDetails = null)
    {
        return new Error
        {
            ErrorCode = $"APP_{errorCode}",
            ErrorMessage = $"[Application:{componentName}] {errorMessage}",
            ErrorDetails = errorDetails ?? []
        };
    }

    /// <summary>
    /// Creates a validation error with consistent formatting.
    /// </summary>
    /// <param name="validationContext">The context where validation failed.</param>
    /// <param name="fieldName">The field that failed validation.</param>
    /// <param name="errorMessage">The validation error message.</param>
    /// <param name="errorDetails">Optional additional details about the validation error.</param>
    /// <returns>An <see cref="IError"/> instance with validation-specific formatting.</returns>
    public static IError CreateValidationError(
        string validationContext,
        string fieldName,
        string errorMessage,
        IList<string>? errorDetails = null)
    {
        return new Error
        {
            ErrorCode = $"VAL_{fieldName.ToUpper()}_INVALID",
            ErrorMessage = $"[Validation:{validationContext}] {errorMessage}",
            ErrorDetails = errorDetails ?? []
        };
    }

    /// <summary>
    /// Creates an exception-based error with consistent formatting.
    /// </summary>
    /// <param name="componentName">The name of the component where the exception occurred.</param>
    /// <param name="exception">The exception that was thrown.</param>
    /// <param name="errorDetails">Optional additional details about the error.</param>
    /// <returns>An <see cref="IError"/> instance with exception-specific formatting.</returns>
    public static IError CreateExceptionError(
        string componentName,
        Exception exception,
        IList<string>? errorDetails = null)
    {
        var details = errorDetails?.ToList() ?? [];
        details.Add($"Exception Type: {exception.GetType().Name}");
        details.Add($"Stack Trace: {exception.StackTrace}");

        if (exception.InnerException != null)
        {
            details.Add($"Inner Exception: {exception.InnerException.Message}");
        }

        return new Error
        {
            ErrorCode = $"EXC_{exception.GetType().Name.ToUpper()}",
            ErrorMessage = $"[Exception:{componentName}] {exception.Message}",
            ErrorDetails = details
        };
    }
}
