using Core.Contracts;
using Domain.Fault;

namespace Core.Models;

/// <summary>
/// Represents the context for analyzing faults, including analysis details, exception information, and
/// customer-specific data.
/// </summary>
/// <remarks>This class aggregates multiple contexts to provide a comprehensive view of fault analysis. It
/// includes the analysis context, exception context, and customer context, which can be used to diagnose and
/// resolve issues effectively.</remarks>
sealed public class FaultAnalysisContext : FaultContext, IFaultAnalysisContext
{
    /// <summary>
    /// Gets or sets the context information related to an exception.
    /// </summary>
    public ExceptionContext ExceptionContext { get; set; } = new();

    /// <summary>
    /// Gets or sets the customer context associated with the current operation.
    /// </summary>
    public CustomerContext CustomerContext { get; set; } = new();
}
