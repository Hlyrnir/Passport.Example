namespace Domain.Interface.Transfer
{
	public interface IPassportHolderTransferObject
	{
		string ConcurrencyStamp { get; init; }
		string CultureName { get; init; }
		string EmailAddress { get; init; }
		bool EmailAddressIsConfirmed { get; init; }
		string FirstName { get; init; }
		Guid Id { get; init; }
		string LastName { get; init; }
		string PhoneNumber { get; init; }
		bool PhoneNumberIsConfirmed { get; init; }
		string SecurityStamp { get; init; }
	}
}
