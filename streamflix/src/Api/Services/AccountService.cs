using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

public interface IAccountService
{
    Task<Account> RegisterAsync(CreateAccountDto dto);
    Task<bool> VerifyAsync(VerifyAccountDto dto);
    Task<string?> LoginAsync(LoginDto dto);
    Task RequestPasswordResetAsync(RequestPasswordResetDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<Account?> GetAccountInfoAsync(string email);
}

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtSerivce _jwtService;

    public AccountService(
        ApplicationDbContext db,
        IPasswordHasherService passwordHasher,
        IJwtSerivce jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Account> RegisterAsync(CreateAccountDto dto)
    {
        if (await _db.Accounts.AnyAsync(a => a.Email == dto.Email))
            throw new InvalidOperationException("Email already in use");

        var account = new Account
        {
            Email = dto.Email,
            Password = _passwordHasher.HashPassword(dto.Password),
            VerificationToken = Guid.NewGuid().ToString(),
            TokenExpire = DateTime.UtcNow.AddHours(24),
            RegistrationDate = DateTime.UtcNow,
            IsActive = true,
            IsVerified = false
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task<bool> VerifyAsync(VerifyAccountDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null || account.IsVerified)
            return false;

        if (account.VerificationToken != dto.VerificationToken)
            return false;

        if (account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow)
            return false;

        account.IsVerified = true;
        account.VerificationToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null || !account.IsVerified)
            return null;

        if (!_passwordHasher.VerifyPassword(account.Password, dto.Password))
            return null;

        account.LastLogin = DateTime.UtcNow;
        account.FailedLoginAttempts = 0;

        await _db.SaveChangesAsync();
        return _jwtService.GenerateToken(account);
    }

    public async Task RequestPasswordResetAsync(RequestPasswordResetDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return;

        account.PasswordResetToken = Guid.NewGuid().ToString();
        account.TokenExpire = DateTime.UtcNow.AddHours(1);

        await _db.SaveChangesAsync();
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return false;

        if (account.PasswordResetToken != dto.PasswordResetToken)
            return false;

        if (account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow)
            return false;

        account.Password = _passwordHasher.HashPassword(dto.NewPassword);
        account.PasswordResetToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Account?> GetAccountInfoAsync(string email)
    {
        return await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
    }
}
