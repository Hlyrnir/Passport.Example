namespace Contract.v01.Request.Authorization.PassportHolder
{
	public sealed class ConfirmEmailAddressRequest
	{
		public required Guid PassportHolderId { get; init; }
		public required string ConcurrencyStamp { get; init; }
		public required string EmailAddress { get; init; }
	}
}
