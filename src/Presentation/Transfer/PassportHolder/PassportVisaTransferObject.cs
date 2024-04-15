namespace Presentation.Transfer.Passport
{
	public struct PassportVisaTransferObject
	{
		public PassportVisaTransferObject()
		{

		}

		public string Id { get; init; } = "DEFAULT_ID";
		public int Level { get; init; } = (-1);
		public string Name { get; init; } = "DEFAULT_NAME";
	}
}
