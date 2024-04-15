namespace Presentation.Transfer.Passport
{
	public struct FindPassportIdByCredentialTransferObject
	{
		public FindPassportIdByCredentialTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string SecurityStamp { get; init; } = "NO_SECURITY_STAMP";

		public string CredentialToFind { get; init; } = "NO_CREDENTIAL";
		public string ProviderOfCredential { get; init; } = "DEFAULT_PROVIDER";
	}
}
