namespace Contract.v01.Request.Authorization.PassportHolder
{
	public sealed class ConfirmPhoneNumberRequest
	{
		public required Guid PassportHolderId { get; init; }
        public required string ConcurrencyStamp { get; init; }
        public required string PhoneNumber { get; init; }
	}
}
