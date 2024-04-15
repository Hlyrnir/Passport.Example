using Domain.Interface.Transfer;

namespace DomainFaker.Implementation
{
    public sealed class PassportTransferObjectFaker : IPassportTransferObject
    {
        public PassportTransferObjectFaker() { }

        public string ConcurrencyStamp { get; init; } = string.Empty;
        public DateTimeOffset ExpiredAt { get; init; }
        public Guid HolderId { get; init; }
        public Guid Id { get; init; }
        public bool IsAuthority { get; init; }
        public bool IsEnabled { get; init; }
        public Guid IssuedBy { get; init; }
        public DateTimeOffset LastCheckedAt { get; init; }
        public Guid LastCheckedBy { get; init; }
        public string Provider { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
	}
}
