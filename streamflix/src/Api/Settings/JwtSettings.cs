namespace Streamflix.Api.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
    }
}
