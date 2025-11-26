namespace Streamflix.Api.DTOs;

public record CreateAccountDto(
    string Email, 
    string Password
);

public record VerifyAccountDto(
    string Email, 
    string VerificationToken
);

public record LoginDto(string Email, 
string Password
);

public record RequestPasswordResetDto(
    string Email
);

public record ResetPasswordDto(
    string Email, string PasswordResetToken, string NewPassword
);

public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public int ExpiryMinutes { get; set; } = 60;
}

