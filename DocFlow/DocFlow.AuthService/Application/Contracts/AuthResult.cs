namespace DocFlow.AuthService.Application.Contracts;

public sealed record AuthResult(bool Success, string Message, AuthResponse? Payload)
{
    public static AuthResult Ok(AuthResponse payload) => new(true, string.Empty, payload);
    public static AuthResult Fail(string message) => new(false, message, null);
}
