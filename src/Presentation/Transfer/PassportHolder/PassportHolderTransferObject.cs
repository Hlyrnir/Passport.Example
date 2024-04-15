namespace Presentation.Transfer.Passport
{
	public struct PassportHolderTransferObject
	{
		public PassportHolderTransferObject()
		{

		}

		public string CultureName { get; init; } = "en-GB";
		public string EmailAddress { get; init; } = "NO_EMAIL_ADDRESS";
		public string FirstName { get; init; } = "NO_FIRST_NAME";
		public string Id { get; init; } = "DEFAULT_ID";
		public string LastName { get; init; } = "NO_LAST_NAME";
		public string PhoneNumber { get; init; } = "NO_PHONE_NUMBER";
	}
}
