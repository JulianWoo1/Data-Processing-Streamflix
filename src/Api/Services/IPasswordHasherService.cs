using Microsoft.AspNetCore.Identity;
using Streamflix.Infrastructure.Entities;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<Account> _hasher = new();

    public string HashPassword(string password)
    {
        var user = new Account();
        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var user = new Account();
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
