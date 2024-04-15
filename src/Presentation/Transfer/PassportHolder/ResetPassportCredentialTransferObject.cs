namespace Presentation.Transfer.Passport
{
	public struct ResetPassportCredentialTransferObject
	{
		public ResetPassportCredentialTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string SignatureToReplace { get; init; } = "DEFAULT_SIGNATURE";
		public string SignatureToApply { get; init; } = "DEFAULT_SIGNATURE";
	}
}
