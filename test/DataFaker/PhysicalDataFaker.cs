using Domain.Interface.PhysicalData;
using Domain.Interface.Transfer;
using DomainFaker.TransferObjectFaker;

namespace DomainFaker
{
	public static partial class DataFaker
	{
		public static class PhysicalDimension
		{
			public static IPhysicalDimension CreateDefault()
			{
				IPhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalData.PhysicalDimension.PhysicalDimension.Create(
				fExponentOfAmpere: 0.0f,
				fExponentOfCandela: 0.0f,
				fExponentOfKelvin: 0.0f,
				fExponentOfKilogram: 0.0f,
				fExponentOfMetre: 0.0f,
				fExponentOfMole: 0.0f,
				fExponentOfSecond: 0.0f,
				dConversionFactorToSI: 1,
				sCultureName: "en-GB",
				sName: "Example",
				sSymbol: "--",
				sUnit: "--");

				if (pdPhysicalDimension is null)
					throw new NullReferenceException();

				return pdPhysicalDimension;
			}

			public static IPhysicalDimension CreateTimeDefault()
			{
				IPhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalData.PhysicalDimension.PhysicalDimension.Create(
				fExponentOfAmpere: 0.0f,
				fExponentOfCandela: 0.0f,
				fExponentOfKelvin: 0.0f,
				fExponentOfKilogram: 0.0f,
				fExponentOfMetre: 0.0f,
				fExponentOfMole: 0.0f,
				fExponentOfSecond: 1.0f,
				dConversionFactorToSI: 1,
				sCultureName: "en-GB",
				sName: "Second",
				sSymbol: "t",
				sUnit: "s");

				if (pdPhysicalDimension is null)
					throw new NullReferenceException();

				return pdPhysicalDimension;
			}

			public static IPhysicalDimension Clone(IPhysicalDimension pdPhysicalDimensionToClone, bool bResetConcurrencyStamp = false)
			{
				string sConcurrencyStamp = pdPhysicalDimensionToClone.ConcurrencyStamp;

				if (bResetConcurrencyStamp == true)
					sConcurrencyStamp = Guid.NewGuid().ToString();

				IPhysicalDimensionTransferObject dtoPhysicalDimensionToClone = pdPhysicalDimensionToClone.WriteTo<PhysicalDimensionTransferObjectFaker>();

				IPhysicalDimensionTransferObject dtoPassport = new PhysicalDimensionTransferObjectFaker()
				{
					ExponentOfAmpere = dtoPhysicalDimensionToClone.ExponentOfAmpere,
					ExponentOfCandela = dtoPhysicalDimensionToClone.ExponentOfCandela,
					ExponentOfKelvin = dtoPhysicalDimensionToClone.ExponentOfKelvin,
					ExponentOfKilogram = dtoPhysicalDimensionToClone.ExponentOfKilogram,
					ExponentOfMetre = dtoPhysicalDimensionToClone.ExponentOfMetre,
					ExponentOfMole = dtoPhysicalDimensionToClone.ExponentOfMole,
					ExponentOfSecond = dtoPhysicalDimensionToClone.ExponentOfSecond,
					ConcurrencyStamp = sConcurrencyStamp,
					ConversionFactorToSI = dtoPhysicalDimensionToClone.ConversionFactorToSI,
					CultureName = dtoPhysicalDimensionToClone.CultureName,
					Id = dtoPhysicalDimensionToClone.Id,
					Name = dtoPhysicalDimensionToClone.Name,
					Symbol = dtoPhysicalDimensionToClone.Symbol,
					Unit = dtoPhysicalDimensionToClone.Unit
				};

				IPhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalData.PhysicalDimension.PhysicalDimension.Initialize(
					dtoPhysicalDimension: dtoPassport);

				if (pdPhysicalDimension is null)
					throw new NullReferenceException();

				return pdPhysicalDimension;
			}
		}

		public static class TimePeriod
		{
			public static ITimePeriod CreateDefault(IPhysicalDimension pdPhysicalDimension)
			{
				ITimePeriod? pdTimePeriod = Domain.Aggregate.PhysicalData.TimePeriod.TimePeriod.Create(
					pdPhysicalDimension: pdPhysicalDimension,
					dMagnitude: new double[] { 0.0, 0.1, 0.2, 0.3, 0.4 },
					dOffset: 0.0);

				if (pdTimePeriod is null)
					throw new NullReferenceException();

				return pdTimePeriod;
			}

			public static ITimePeriod Clone(ITimePeriod pdTimePeriodToClone, bool bResetConcurrencyStamp = false)
			{
				string sConcurrencyStamp = pdTimePeriodToClone.ConcurrencyStamp;

				if (bResetConcurrencyStamp == true)
					sConcurrencyStamp = Guid.NewGuid().ToString();

				ITimePeriodTransferObject dtoTimePeriodToClone = pdTimePeriodToClone.WriteTo<TimePeriodTransferObjectFaker>();

				ITimePeriodTransferObject dtoTimePeriod = new TimePeriodTransferObjectFaker()
				{
					ConcurrencyStamp = sConcurrencyStamp,
					Id = dtoTimePeriodToClone.Id,
					Magnitude = dtoTimePeriodToClone.Magnitude,
					Offset = dtoTimePeriodToClone.Offset,
					PhysicalDimensionId = dtoTimePeriodToClone.PhysicalDimensionId
				};

				ITimePeriod? pdTimePeriod = Domain.Aggregate.PhysicalData.TimePeriod.TimePeriod.Initialize(
					dtoTimePeriod: dtoTimePeriod);

				if (pdTimePeriod is null)
					throw new NullReferenceException();

				return pdTimePeriod;
			}
		}
	}
}
