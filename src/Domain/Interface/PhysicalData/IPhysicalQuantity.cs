namespace Domain.Interface.PhysicalData
{
	public interface IPhysicalQuantity<T> where T : notnull
	{
		string ConcurrencyStamp { get; }

		Guid Id { get; }
		Guid PhysicalDimensionId { get; }
		T Magnitude { get; set; }

		bool TryChangePhysicalDimension(IPhysicalDimension pdPhysicalDimension);
	}
}