namespace Presentation.Transfer.Passport
{
	public struct ResetPassportCredentialByIdTransferObject
	{
		public ResetPassportCredentialByIdTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string SecurityStamp { get; init; } = "NO_SECURITY_STAMP";

		public string PassportId { get; init; } = "DEFAULT_ID";
		public string ProviderToApplyAt { get; init; } = "DEFAULT_PROVIDER";
		public string CredentialToApply { get; init; } = "DEFAULT_CREDENTIAL";
		public string SignatureToApply { get; init; } = "DEFAULT_SIGNATURE";
	}
}
