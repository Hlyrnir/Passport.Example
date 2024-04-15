namespace Domain.Interface.Transfer
{
	public interface IPassportVisaTransferObject
	{
		string ConcurrencyStamp { get; init; }
		Guid Id { get; init; }
		int Level { get; init; }
		string Name { get; init; }
	}
}
