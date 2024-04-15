using Domain.Interface.Transfer;

namespace Domain.Interface.Authorization
{
	public interface IPassportVisa
	{
		string ConcurrencyStamp { get; }
		public Guid Id { get; }
		public string Name { get; }
		public int Level { get; }

		bool TryChangeName(string sName);
		bool TryChangeLevel(int iLevel);

		IPassportVisaTransferObject WriteTo<T>() where T : IPassportVisaTransferObject, new();
	}
}
