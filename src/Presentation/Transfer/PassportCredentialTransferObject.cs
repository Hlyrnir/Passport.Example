namespace Presentation.Transfer.Passport
{
	public class PassportCredentialTransferObject
	{
		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "NO_CREDENTIAL";
		public string Signature { get; init; } = "NO_SIGNATURE";
	}
}
