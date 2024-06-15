namespace Contract.v01.Response.Passport
{
	public sealed class PassportVisaResponse
	{
        public required string ConcurrencyStamp { get; init; }
        public required Guid Id { get; init; }
		public required string Name { get; init; }
		public required int Level { get; init; }
	}
}
