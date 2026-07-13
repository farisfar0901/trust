namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// The outcome of a login attempt. Login has more than one meaningful
/// failure reason, so this result type is used instead of throwing an
/// exception for an expected, routine outcome like a wrong password.
/// </summary>
public sealed class LoginResult
{
    /// <summary>
    /// Whether the login attempt succeeded.
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// The reason the attempt failed. Only set when <see cref="IsSuccess"/> is <see langword="false"/>.
    /// </summary>
    public LoginFailureReason? FailureReason { get; init; }

    /// <summary>
    /// The issued tokens and profile summary. Only set when <see cref="IsSuccess"/> is <see langword="true"/>.
    /// </summary>
    public LoginResponse? Response { get; init; }

    /// <summary>
    /// Creates a successful result carrying the issued tokens.
    /// </summary>
    /// <param name="response">The tokens and profile summary to return to the caller.</param>
    public static LoginResult Success(LoginResponse response)
        => new() { IsSuccess = true, Response = response };

    /// <summary>
    /// Creates a failed result carrying the reason for the failure.
    /// </summary>
    /// <param name="reason">Why the login attempt did not succeed.</param>
    public static LoginResult Failure(LoginFailureReason reason)
        => new() { IsSuccess = false, FailureReason = reason };
}
