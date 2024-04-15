using Domain.Interface.Transfer;

namespace Infrastructure.Transfer.PhysicalData
{
	internal sealed class PhysicalDimensionTransferObject : IPhysicalDimensionTransferObject
	{
		public float ExponentOfAmpere { get;init; }
		public float ExponentOfCandela { get; init; }
		public float ExponentOfKelvin { get; init; }
		public float ExponentOfKilogram { get; init; }
		public float ExponentOfMetre { get; init; }
		public float ExponentOfMole { get; init; }
		public float ExponentOfSecond { get; init; }
		public string ConcurrencyStamp { get; init; } = string.Empty;
		public double ConversionFactorToSI { get; init; }
		public string CultureName { get; init; } = string.Empty;
		public Guid Id { get; init; }
		public string Name { get; init; } = string.Empty;
		public string Symbol { get; init; } = string.Empty;
		public string Unit { get; init; } = string.Empty;
	}
}
