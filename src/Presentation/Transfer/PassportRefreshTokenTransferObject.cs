namespace Presentation.Transfer.Passport
{
	public class PassportRefreshTokenTransferObject
	{
		public Guid PassportId { get; init; } = Guid.Empty;
		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string RefreshToken { get; init; } = "NO_REFRESH_TOKEN";
	}
}
