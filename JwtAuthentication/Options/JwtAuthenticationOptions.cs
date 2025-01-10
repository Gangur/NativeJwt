namespace JwtAuthentication.Options
{
    public sealed record JwtAuthenticationOptions
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required string Secret { get; init; }
        public required TimeSpan ExpirationPeriod { get; init; }
    }
}
