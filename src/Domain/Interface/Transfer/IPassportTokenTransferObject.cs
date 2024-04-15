namespace Domain.Interface.Transfer
{
	public interface IPassportTokenTransferObject
	{
		public Guid Id { get; init; }
		public Guid PassportId { get; init; }
		public string Provider { get; init; }
		public string RefreshToken { get; init; }
		public bool TwoFactorIsEnabled { get; init; }
	}
}
