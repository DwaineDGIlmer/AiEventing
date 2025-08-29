using Core.Contracts;
using Domain.Analysis;

namespace Core.Models;

/// <summary>
/// Represents an error with a code, message, and optional details.
/// </summary>
public class Error : Errors, IError { }
