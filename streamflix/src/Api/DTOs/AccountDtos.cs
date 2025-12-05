using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

public record CreateAccountDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    string Password   
);

public record VerifyAccountDto(
    [Required] string Email, 
    [Required] string VerificationToken
);

public record LoginDto(
    [Required] string Email, 
    [Required] string Password
);

public record LoginResult(
    bool Success,
    string? Token,
    string? ErrorMessage
);


public record RequestPasswordResetDto(
    [Required] string Email
);

public record ResetPasswordDto(
    [Required] string Email, 
    [Required] string PasswordResetToken, 
    [Required] string NewPassword
);

public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public int ExpiryMinutes { get; set; } = 60;
}

