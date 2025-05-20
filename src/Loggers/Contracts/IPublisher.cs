namespace Loggers.Contracts;

/// <summary>
/// Defines a contract for publishing messages to an output writer.
/// </summary>
/// <remarks>Implementations of this interface are responsible for writing messages to a designated output, such
/// as a console, file, or network stream. The methods provided allow for writing messages with or without a line
/// terminator.</remarks>
public interface IPublisher
{
    /// <summary>
    /// Writes a message to this instance's Writer.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    Task WriteLine(string message);

    /// <summary>
    /// Writes a message to this instance's Writer followed by a line terminator. 
    /// The default line terminator is a carriage return followed by a line feed (\r\n).
    /// </summary>
    /// <param name="message">The message to publish.</param>
    Task Write(string message);
}
