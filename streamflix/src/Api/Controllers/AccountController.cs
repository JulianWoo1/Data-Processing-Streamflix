using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Streamflix.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/xml", "text/csv")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IWebHostEnvironment _hostEnvironment;

    public AccountController(IAccountService accountService, IWebHostEnvironment hostEnvironment)
    {
        _accountService = accountService;
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (!new EmailAddressAttribute().IsValid(dto.Email))
        {
            return BadRequest(new { Message = "Invalid email format." });
        }
        try
        {
            var result = await _accountService.RegisterAsync(dto);

            return Ok(new
            {
                result.account.AccountId,
                result.account.Email,
                VerificationToken = result.verificationToken
            });
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
        var result = await _accountService.LoginAsync(dto);
        return result.Success
            ? Ok(new { Token = result.Token })
            : Unauthorized(result.ErrorMessage);
    }

    [HttpPost("requestPasswordReset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        var resetToken = await _accountService.RequestPasswordResetAsync(dto);

        return Ok(new
        {
            Message = resetToken != null
                ? "Password reset token generated successfully."
                : "If an account exists, a reset link was sent.",
            PasswordResetToken = resetToken
        });
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var success = await _accountService.ResetPasswordAsync(dto);
        return success ? Ok("Password reset successfully.") : BadRequest("Reset failed.");
    }

    [Authorize]
    [HttpGet("getAccountInfo")]
    public async Task<IActionResult> GetAccountInfo([FromQuery] string email)
    {
        var account = await _accountService.GetAccountInfoAsync(email);
        if (account == null)
        {
            return NotFound();
        }

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
