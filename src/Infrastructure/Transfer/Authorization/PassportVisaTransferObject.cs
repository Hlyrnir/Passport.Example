using Domain.Interface.Transfer;

namespace Infrastructure.Transfer.Authorization
{
    internal sealed class PassportVisaTransferObject : IPassportVisaTransferObject
    {
        public string ConcurrencyStamp { get; init; } = string.Empty;
        public Guid Id { get; init; }
        public int Level { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
