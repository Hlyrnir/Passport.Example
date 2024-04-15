namespace Presentation.Transfer.Passport
{
	public struct UpdatePassportHolderTransferObject
	{
		public UpdatePassportHolderTransferObject()
		{

		}

		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Provider { get; init; } = "DEFAULT_JWT";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string CultureName { get; init; } = "en-GB";
		public string EmailAddress { get; init; } = "NO_EMAIL_ADDRESS";
		public string FirstName { get; init; } = "NO_FIRST_NAME";
		public string Id { get; init; } = "DEFAULT_ID";
		public string LastName { get; init; } = "NO_LAST_NAME";
		public string PhoneNumber { get; init; } = "NO_PHONE_NUMBER";
	}
}