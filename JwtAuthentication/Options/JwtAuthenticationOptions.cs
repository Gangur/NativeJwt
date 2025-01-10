namespace JwtAuthentication.Options
{
    internal sealed record JwtAuthenticationOptions
    {
        internal string Issuer { get; init; } = string.Empty;
        internal string Audience { get; init; } = string.Empty;
        internal string Secret { get; init; } = string.Empty;
        internal TimeSpan ExpirationPeriod { get; init; }
    }
}
