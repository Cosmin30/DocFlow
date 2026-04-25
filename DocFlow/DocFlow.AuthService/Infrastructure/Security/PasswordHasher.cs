namespace DocFlow.AuthService.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string value) => BCrypt.Net.BCrypt.EnhancedHashPassword(value, 13);
    public bool Verify(string value, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(value, hash);
}
