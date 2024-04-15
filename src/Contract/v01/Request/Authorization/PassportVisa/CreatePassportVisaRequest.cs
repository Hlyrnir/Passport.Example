namespace Contract.v01.Request.Authorization.PassportVisa
{
	public sealed class CreatePassportVisaRequest
	{
		public required string Name { get; init; }
		public required int Level { get; init; }
	}
}
