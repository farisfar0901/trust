namespace Trust.Api.Application.Common;

/// <summary>
/// Provides the current date and time. Injected instead of calling
/// <see cref="DateTimeOffset.UtcNow"/> directly so that services and
/// repositories can be unit tested with a fixed, predictable clock.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current moment in time, expressed as UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
