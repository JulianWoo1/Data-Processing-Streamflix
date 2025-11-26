namespace Streamflix.Infrastructure.Entities;

public class Account
{
    public int AccountId { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime BlockedUntil { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? TokenExpire { get; set; }
    public string? PasswordResetToken { get; set; }
}
