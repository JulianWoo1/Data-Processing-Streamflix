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
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountDto dto)
    {
        try
        {
            var account = await _accountService.RegisterAsync(dto);
            return Ok(new { account.AccountId, account.Email });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyAccountDto dto)
    {
        var success = await _accountService.VerifyAsync(dto);
        return success ? Ok("Account verified successfully") : BadRequest("Verification failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _accountService.LoginAsync(dto);
        return token == null ? Unauthorized("Invalid credentials.") : Ok(new { Token = token });
    }

    [HttpPost("requestPasswordReset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        await _accountService.RequestPasswordResetAsync(dto);
        return Ok("If an account exists, a reset link was sent.");
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var success = await _accountService.ResetPasswordAsync(dto);
        return success ? Ok("Password reset successfully.") : BadRequest("Reset failed.");
    }

    [HttpGet("getAccountInfo")]
    public async Task<IActionResult> GetAccountInfo([FromQuery] string email)
    {
        var account = await _accountService.GetAccountInfoAsync(email);
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
