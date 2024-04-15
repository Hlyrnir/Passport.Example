namespace Presentation.Transfer.Passport
{
	public struct RegisterPassportTransferObject
	{
		public RegisterPassportTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string SecurityStamp { get; init; } = "NO_SECURITY_STAMP";

		public string CultureName { get; init; } = "en-GB";
		public string EmailAddress { get; init; } = "NO_EMAIL_ADDRESS";
		public string FirstName { get; init; } = "NO_FIRST_NAME";
		public string LastName { get; init; } = "NO_LAST_NAME";
		public string PhoneNumber { get; init; } = "NO_PHONE_NUMBER";

		public string ProviderToRegisterAt { get; init; } = "DEFAULT_PROVIDER";
		public string CredentialToRegister { get; init; } = "DEFAULT_CREDENTIAL";
		public string SignatureToRegister { get; init; } = "DEFAULT_SIGNATURE";
	}
}
