namespace Presentation.Transfer.Passport
{
	public struct PassportByIdTransferObject
	{
		public PassportByIdTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "NO_CREDENTIAL";
		public string Signature { get; init; } = "NO_SIGNATURE";

		public string SecurityStamp { get; init; } = "NO_SECURITY_STAMP";

		public string PassportId { get; init; } = "NO_PASSPORT_ID";
	}
}
