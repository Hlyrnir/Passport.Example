using Domain.Interface.Transfer;

namespace Domain.Interface.Authorization
{
	public interface IPassportToken
	{
		public Guid Id { get; }
		public Guid PassportId { get; }
		public string Provider { get; }
		public string RefreshToken { get; }
		public bool TwoFactorIsEnabled { get; }

		bool TryEnableTwoFactorAuthentication(bool bEnable = false);

		IPassportTokenTransferObject WriteTo<T>() where T : IPassportTokenTransferObject, new();
	}
}
