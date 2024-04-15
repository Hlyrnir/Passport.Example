namespace Contract.v01.Request.Authorization.PassportVisa
{
	public class UpdatePassportVisaRequest
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
		public required int Level { get; init; }
	}
}
