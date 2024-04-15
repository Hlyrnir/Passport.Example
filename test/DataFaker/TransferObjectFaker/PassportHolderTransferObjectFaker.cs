using Domain.Interface.Transfer;

namespace DomainFaker.Implementation
{
	internal class PassportHolderTransferObjectFaker : IPassportHolderTransferObject
	{
		public PassportHolderTransferObjectFaker() { }

		public string ConcurrencyStamp { get; init; } = string.Empty;
		public string CultureName { get; init; } = string.Empty;
		public string EmailAddress { get; init; } = string.Empty;
		public bool EmailAddressIsConfirmed { get; init; }
		public string FirstName { get; init; } = string.Empty;
		public Guid Id { get; init; }
		public string LastName { get; init; } = string.Empty;
		public string PhoneNumber { get; init; } = string.Empty;
		public bool PhoneNumberIsConfirmed { get; init; }
		public string SecurityStamp { get; init; } = string.Empty;
	}
}
