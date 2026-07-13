namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// The outcome of a token refresh attempt.
/// </summary>
public sealed class RefreshTokenResult
{
    /// <summary>
    /// Whether the refresh attempt succeeded.
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// The reason the attempt failed. Only set when <see cref="IsSuccess"/> is <see langword="false"/>.
    /// </summary>
    public RefreshTokenFailureReason? FailureReason { get; init; }

    /// <summary>
    /// The newly issued tokens. Only set when <see cref="IsSuccess"/> is <see langword="true"/>.
    /// </summary>
    public RefreshTokenResponse? Response { get; init; }

    /// <summary>
    /// Creates a successful result carrying the newly issued tokens.
    /// </summary>
    /// <param name="response">The new tokens to return to the caller.</param>
    public static RefreshTokenResult Success(RefreshTokenResponse response)
        => new() { IsSuccess = true, Response = response };

    /// <summary>
    /// Creates a failed result carrying the reason for the failure.
    /// </summary>
    /// <param name="reason">Why the refresh attempt did not succeed.</param>
    public static RefreshTokenResult Failure(RefreshTokenFailureReason reason)
        => new() { IsSuccess = false, FailureReason = reason };
}
