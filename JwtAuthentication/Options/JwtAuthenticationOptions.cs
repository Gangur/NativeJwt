namespace JwtAuthentication.Options
{
    /// <summary>
    /// JWT authentication options from configuration in appsettings.json, section JwtAuthentication
    /// </summary>
    public sealed record JwtAuthenticationOptions
    {
        /// <summary>
        /// Url where jwt was issued
        /// </summary>
        public required string Issuer { get; init; }

        /// <summary>
        /// Url of jwt autience
        /// </summary>
        public required string Audience { get; init; }

        /// <summary>
        /// Secret, long enough to produce 512 encryption
        /// </summary>
        public required string Secret { get; init; }

        /// <summary>
        /// Expiration period in format d.hh.mm.ss
        /// </summary>
        public required TimeSpan ExpirationPeriod { get; init; }
    }
}
