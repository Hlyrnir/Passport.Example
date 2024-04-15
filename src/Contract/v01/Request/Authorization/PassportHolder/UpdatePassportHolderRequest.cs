namespace Contract.v01.Request.Authorization.PassportHolder
{
	public sealed class UpdatePassportHolderRequest
	{
		public required Guid PassportHolderId { get; init; }
		public required string CultureName { get; init; }
		public required string EmailAddress { get; init; }
		public required string FirstName { get; init; }
		public required string LastName { get; init; }
		public required string PhoneNumber { get; init; }
	}
}
