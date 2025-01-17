namespace Domain.Options
{
    public class AuthOptions
    {
        public required string TokenPrivateKey { get; set; }
        public int ExpiresIntervalMinutes { get; set; }
    }
}
