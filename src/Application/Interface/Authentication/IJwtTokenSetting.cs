namespace Application.Interface.Authentication
{
    public interface IJwtTokenSetting
    {
		string Type { get; }
		string Audience { get; init; }
        string Issuer { get; init; }
        string SecretKey { get; init; }
    }
}