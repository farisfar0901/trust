namespace Trust.Api.Application.Common;

/// <summary>
/// Default <see cref="IDateTimeProvider"/> implementation that reads the
/// real system clock. Registered for normal application use; tests can
/// substitute a fake implementation instead.
/// </summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
