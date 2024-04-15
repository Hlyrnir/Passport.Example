using Domain.Interface.Transfer;

namespace DomainFaker.Implementation
{
	internal class PassportTokenTransferObjectFaker : IPassportTokenTransferObject
	{
		public Guid Id { get;init; }
		public Guid PassportId { get; init; }
		public string Provider { get; init; } = string.Empty;
		public string RefreshToken { get; init; } = string.Empty;
		public bool TwoFactorIsEnabled { get; init; }
	}
}
