using Domain.Interface.Transfer;

namespace Domain.Interface.Authorization
{
	public interface IPassportHolder
	{
		string ConcurrencyStamp { get; }
		string CultureName { get; }
		string EmailAddress { get; }
		bool EmailAddressIsConfirmed { get; }
		string FirstName { get; set; }
		Guid Id { get; }
		string LastName { get; set; }
		string PhoneNumber { get; }
		bool PhoneNumberIsConfirmed { get; }
		string SecurityStamp { get; }

		bool TryChangeCultureName(string sCultureName);
		bool TryChangeEmailAddress(string sEmailAddress, IPassportSetting ppSetting);
		bool TryChangePhoneNumber(string sPhoneNumber, IPassportSetting ppSetting);
		bool TryConfirmEmailAddress(string sEmailAddressToConfirm, IPassportSetting ppSetting);
		bool TryConfirmPhoneNumber(string sPhoneNumberToConfirm, IPassportSetting ppSetting);

		IPassportHolderTransferObject WriteTo<T>() where T : IPassportHolderTransferObject, new();
	}
}