using Domain.Interface.Transfer;

namespace Domain.Interface.Authorization
{
	public interface IPassport
	{
		string ConcurrencyStamp { get; }
		DateTimeOffset ExpiredAt { get; }
		Guid HolderId { get; }
		Guid Id { get; }
		bool IsAuthority { get; }
		bool IsEnabled { get; }
		Guid IssuedBy { get; }
		DateTimeOffset LastCheckedAt { get; }
		Guid LastCheckedBy { get; }
		IEnumerable<Guid> VisaId { get; }
		
		bool TryDisable(IPassport ppPassport, DateTimeOffset dtDate);
		bool TryEnable(IPassport ppPassport, DateTimeOffset dtEnabledAt);
		bool TryExtendTerm(DateTimeOffset dtDate, DateTimeOffset dtCheckedAt, Guid guCheckedBy);
		bool HasVisa(IPassportVisa ppVisa);
		bool IsExpired(DateTimeOffset dtDate);
		bool TryJoinToAuthority(IPassport ppPassport, DateTimeOffset dtJointedAt);
		bool TryReset(IPassport ppPassport, DateTimeOffset dtResetAt);
		bool TryAddVisa(IPassportVisa ppVisa);
		bool TryRemoveVisa(IPassportVisa ppVisa);

		IPassportTransferObject WriteTo<T>() where T : IPassportTransferObject, new();
	}
}