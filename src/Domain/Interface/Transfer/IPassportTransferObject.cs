namespace Domain.Interface.Transfer
{
	public interface IPassportTransferObject
	{
		string ConcurrencyStamp { get; init; }
		public DateTimeOffset ExpiredAt { get; init; }
		Guid HolderId { get; init; }
		public Guid Id { get; init; }
		public bool IsAuthority { get; init; }
		public bool IsEnabled { get; init; }
		public Guid IssuedBy { get; init; }
		public DateTimeOffset LastCheckedAt { get; init; }
		public Guid LastCheckedBy { get; init; }
	}
}
