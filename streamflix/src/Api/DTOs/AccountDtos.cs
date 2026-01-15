using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

public class CreateAccountDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string Password { get; set; }

    public CreateAccountDto()
    {
        Email = string.Empty;
        Password = string.Empty;
    }
}

public class VerifyAccountDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string VerificationToken { get; set; }

    public VerifyAccountDto()
    {
        Email = string.Empty;
        VerificationToken = string.Empty;
    }
}

public class LoginDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public LoginDto()
    {
        Email = string.Empty;
        Password = string.Empty;
    }
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
}


public class RequestPasswordResetDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public RequestPasswordResetDto()
    {
        Email = string.Empty;
    }
}

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PasswordResetToken { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string NewPassword { get; set; }

    public ResetPasswordDto()
    {
        Email = string.Empty;
        PasswordResetToken = string.Empty;
        NewPassword = string.Empty;
    }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public int ExpiryMinutes { get; set; } = 60;
}
