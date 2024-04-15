using Domain.Interface.Transfer;

namespace DomainFaker.Implementation
{
	internal class PassportVisaTransferObjectFaker : IPassportVisaTransferObject
	{
		public string ConcurrencyStamp { get; init; } = string.Empty;
		public Guid Id { get;init; }
		public int Level { get;init; }
		public string Name { get; init; } = string.Empty;
	}
}
