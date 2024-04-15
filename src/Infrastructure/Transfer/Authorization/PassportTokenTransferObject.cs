using Domain.Interface.Transfer;

namespace Infrastructure.Transfer.Authorization
{
	internal sealed class PassportTokenTransferObject : IPassportTokenTransferObject
	{
		public required Guid Id { get; init; }
		public required Guid PassportId { get; init; }
		public required string Provider { get; init; }
		public required string RefreshToken { get; init; }
		public required bool TwoFactorIsEnabled { get; init; }
	}
}