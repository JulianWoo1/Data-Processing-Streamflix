using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public AccountController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountDto dto)
    {
        if (await _db.Accounts.AnyAsync(a => a.Email == dto.Email))
            return BadRequest("Email already in use.");

        var account = new Account
        {
            Email = dto.Email,
            Password = dto.Password,
            VerificationToken = Guid.NewGuid().ToString(),
            TokenExpire = DateTime.UtcNow.AddHours(24),
            RegistrationDate = DateTime.UtcNow,
            IsActive = true,
            IsVerified = false
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        return Ok(new { account.AccountId, account.Email });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyAccountDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return NotFound("Account not found");
        if (account.IsVerified) return BadRequest("Account is already verified.");
        if (account.VerificationToken != dto.VerificationToken) return BadRequest("Invalid token.");
        if (account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow) return BadRequest("Token expired.");

        account.IsVerified = true;
        account.VerificationToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return Ok("Account verified successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return Unauthorized("Invalid credentials.");
        if (!account.IsVerified) return Unauthorized("Account is not verified.");
        if (account.Password != dto.Password) return Unauthorized("Invalid credentials.");

        account.LastLogin = DateTime.UtcNow;
        account.FailedLoginAttempts = 0;
        await _db.SaveChangesAsync();

        return Ok(new { account.AccountId, account.Email });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok("Logged out successfully");
    }

    [HttpPost("requestPasswordReset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return Ok();

        account.PasswordResetToken = Guid.NewGuid().ToString();
        account.TokenExpire = DateTime.UtcNow.AddHours(1);

        await _db.SaveChangesAsync();
        return Ok("If an account exists, a reset link was sent.");
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (account == null) return BadRequest("Invalid request.");
        if (account.PasswordResetToken == null || account.PasswordResetToken != dto.PasswordResetToken) return BadRequest("Invalid token.");
        if (account.TokenExpire == null || account.TokenExpire < DateTime.UtcNow) return BadRequest("Token expired.");

        account.Password = dto.NewPassword;
        account.PasswordResetToken = null;
        account.TokenExpire = null;

        await _db.SaveChangesAsync();
        return Ok("Password reset successfully.");
    }

    [HttpGet("getAccountInfo")]
    public async Task<IActionResult> GetAccountInfo([FromQuery] string email)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        if (account == null) return NotFound();

        return Ok(new
        {
            account.AccountId,
            account.Email,
            account.RegistrationDate,
            account.LastLogin,
            account.IsActive,
            account.IsVerified
        });
    }
}
