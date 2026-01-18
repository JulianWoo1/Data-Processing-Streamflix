using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;
using Streamflix.Api.Services;

public interface IAccountService
{
    Task<(Account account, string verificationToken)> RegisterAsync(CreateAccountDto dto);
    Task<bool> VerifyAsync(VerifyAccountDto dto);
    Task<LoginResult> LoginAsync(LoginDto dto);
    Task<string?> RequestPasswordResetAsync(RequestPasswordResetDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<Account?> GetAccountInfoAsync(string email);
}

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtService _IJwtService;

    public AccountService(
        ApplicationDbContext db,
        IPasswordHasherService passwordHasher,
        IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _IJwtService = jwtService;
    }

    public async Task<(Account account, string verificationToken)> RegisterAsync(CreateAccountDto dto)
    {
        var normalizedEmail = dto.Email.ToLowerInvariant();

        if (await _db.Accounts.AnyAsync(a => a.Email.ToLower() == normalizedEmail))
        {
            throw new InvalidOperationException("Email already in use");
        }

        var verificationToken = Guid.NewGuid().ToString();

        var account = new Account
        {
            Email = normalizedEmail,
            Password = _passwordHasher.HashPassword(dto.Password),
            VerificationToken = verificationToken,
            TokenExpire = DateTime.UtcNow.AddHours(24),
            RegistrationDate = DateTime.UtcNow,
            IsActive = true,
            IsVerified = false
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return (account, verificationToken);
    }

    public async Task<bool> VerifyAsync(VerifyAccountDto dto)
    {
        var normalizedEmail = dto.Email.ToLowerInvariant();
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

        if (account == null || account.IsVerified)
        {
            return false;
        }

        if (account.VerificationToken != dto.VerificationToken || account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow)
        {
            return false;
        }

        account.IsVerified = true;
        account.VerificationToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResult> LoginAsync(LoginDto dto)
    {
        var normalizedEmail = dto.Email.ToLowerInvariant();
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

        if (account == null)
        {
            return new LoginResult{ Success = false, Token = null, ErrorMessage = "Invalid credentials."};
        }

        if (!account.IsVerified)
        {
            return new LoginResult{ Success = false, Token = null, ErrorMessage = "Account not verified. Please verify your email."};
        }

        if (account.BlockedUntil > DateTime.UtcNow)
        {
            var minutesLeft = (int)(account.BlockedUntil - DateTime.UtcNow).TotalMinutes;
            return new LoginResult{ Success = false, Token = null, ErrorMessage = $"Account temporarily locked. Try again in {minutesLeft} minutes."};
        }

        if (!_passwordHasher.VerifyPassword(account.Password, dto.Password))
        {
            account.FailedLoginAttempts++;

            const int maxAttempts = 3;
            const int blockMinutes = 60;

            if (account.FailedLoginAttempts >= maxAttempts)
            {
                account.BlockedUntil = DateTime.UtcNow.AddMinutes(blockMinutes);
                account.FailedLoginAttempts = 0;
            }

            await _db.SaveChangesAsync();
            return new LoginResult{ Success = false, Token = null, ErrorMessage = "Invalid credentials."};
        }

        account.FailedLoginAttempts = 0;
        account.BlockedUntil = DateTime.MinValue;
        account.LastLogin = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var token = _IJwtService.GenerateToken(account);
        return new LoginResult{ Success = true, Token = token, ErrorMessage = null};
    }


    public async Task<string?> RequestPasswordResetAsync(RequestPasswordResetDto dto)
    {
        var normalizedEmail = dto.Email.ToLowerInvariant();
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

        if (account == null)
        {
            return null;
        }
        var resetToken = Guid.NewGuid().ToString();
        account.PasswordResetToken = resetToken;
        account.TokenExpire = DateTime.UtcNow.AddHours(1);

        await _db.SaveChangesAsync();
        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var normalizedEmail = dto.Email.ToLowerInvariant();
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

        if (account == null)
        {
            return false;
        }

        if (account.PasswordResetToken != dto.PasswordResetToken ||
            account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow)
        {
            return false;
        }

        account.Password = _passwordHasher.HashPassword(dto.NewPassword);
        account.PasswordResetToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return true;
    }


    public async Task<Account?> GetAccountInfoAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _db.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);
    }
}
