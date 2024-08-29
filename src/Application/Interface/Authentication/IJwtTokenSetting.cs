namespace Application.Interface.Authentication
{
    internal interface IJwtTokenSetting
    {
		string Type { get; }
		string Audience { get; init; }
        string Issuer { get; init; }
        string SecretKey { get; init; }

		int LifetimeInMinutes { get; init; }
	}
}