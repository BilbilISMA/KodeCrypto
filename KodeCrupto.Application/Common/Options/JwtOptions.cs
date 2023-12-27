using System;
namespace KodeCrypto.Application.Common.Options
{
	public class JwtOptions
	{
        public const string SettingsSection = "jwt";

        public string SecretKey { get; set; }
        public int ExpiryMinutesAccessToken { get; set; }
        public int ExpiryMinutesRefreshToken { get; set; }
        public string Issuer { get; set; }
    }
}

