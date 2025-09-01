using Core.Contracts;
using Domain.Analysis;

namespace Core.Models;

/// <summary>
/// Represents an error with a code, message, and optional details.
/// </summary>
public sealed class Error : Errors, IError { }
