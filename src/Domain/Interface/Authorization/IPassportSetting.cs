namespace Domain.Interface.Authorization
{
	public interface IPassportSetting
	{
		TimeSpan ExpiresAfterDuration { get; init; }
		int MaximalAllowedAccessAttempt { get; init; }
		int MaximalCredentialLength { get; init; }
		int MaximalSignatureLength { get; init; }
		TimeSpan MaximalRefreshTokenEffectivity { get; init; }
		TimeSpan MinimalDelayBetweenAttempt { get; init; }
		TimeSpan MinimalLockoutDuration { get; init; }
		int MinimalPhoneNumberLength { get; init; }
		int RequiredMinimalCredentialLength { get; init; }
		int RequiredMinimalSignatureLength { get; init; }
		string RequiredDigit { get; }
		string RequiredLowerCase { get; }
		string RequiredUpperCase { get; }
		string RequiredSpecial { get; init; }
		bool TwoFactorAuthentication { get; init; }
		IEnumerable<string> ValidProviderName { get; init; }
	}
}