namespace Presentation.Transfer.Passport
{
	public struct IssuePassportVisaTransferObject
	{
		public IssuePassportVisaTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string Name { get; init; } = "DEFAULT_ID";
		public int Level { get; init; } = (-1);
	}
}
