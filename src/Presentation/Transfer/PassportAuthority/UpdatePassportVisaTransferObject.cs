namespace Presentation.Transfer.Passport
{
	public struct UpdatePassportVisaTransferObject
	{
		public UpdatePassportVisaTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string Id { get; init; } = "DEFAULT_ID";
		public string Name { get; init; } = "DEFAULT_ID";
		public int Level { get; init; } = (-1);
	}
}
