namespace Domain.Interface.Transfer
{
	public interface IPhysicalDimensionTransferObject
	{
		float ExponentOfAmpere { get; init; }

		float ExponentOfCandela { get; init; }

		float ExponentOfKelvin { get; init; }

		float ExponentOfKilogram { get; init; }

		float ExponentOfMetre { get; init; }

		float ExponentOfMole { get; init; }

		float ExponentOfSecond { get; init; }

		string ConcurrencyStamp { get; init; }

		double ConversionFactorToSI { get; init; }

		string CultureName { get; init; }
		Guid Id { get; init; }
		string Name { get; init; }

		string Symbol { get; init; }

		string Unit { get; init; }
	}
}