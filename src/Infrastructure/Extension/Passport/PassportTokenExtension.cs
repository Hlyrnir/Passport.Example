namespace Infrastructure.Extension.Passport
{
	public enum PassportTokenColumn
	{
		CreatedAt,
		Credential,
		EditedAt,
		FailedAttemptCounter,
		Id,
		LastFailedAt,
		PassportId,
		Provider,
		RefreshToken,
		Signature,
		TwoFactorIsEnabled
	}

	public enum PassportTokenTable
	{
		PassportToken
	}
}
