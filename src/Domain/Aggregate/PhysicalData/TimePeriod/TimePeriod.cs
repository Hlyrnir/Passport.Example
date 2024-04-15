using Domain.Interface.PhysicalData;
using Domain.Interface.Transfer;

namespace Domain.Aggregate.PhysicalData.TimePeriod
{
	public sealed class TimePeriod : ITimePeriod
	{
		private string sConcurrencyStamp;
		private Guid guId;

		private Guid guPhysicalDimensionId;
		private double[] dMagnitude;
		private double dOffset;

		private TimePeriod(
			string sConcurrencyStamp,
			Guid guId,
			Guid guPhysicalDimensionId,
			double[] dMagnitude,
			double dOffset)
		{
			this.sConcurrencyStamp = sConcurrencyStamp;
			this.guId = guId;
			this.guPhysicalDimensionId = guPhysicalDimensionId;
			this.dMagnitude = dMagnitude;
			this.dOffset = dOffset;
		}

		/// <inheritdoc/>
		public string ConcurrencyStamp { get => sConcurrencyStamp; private set => sConcurrencyStamp = value; }

		/// <inheritdoc/>
		public Guid Id { get => guId; private set => guId = value; }

		/// <inheritdoc/>
		public double[] Magnitude { get => dMagnitude; set => dMagnitude = value; }

		/// <inheritdoc/>
		public double Offset { get => dOffset; set => dOffset = value; }

		/// <inheritdoc/>
		public Guid PhysicalDimensionId { get => guPhysicalDimensionId; private set => guPhysicalDimensionId = value; }

		/// <inheritdoc/>
		public bool TryChangePhysicalDimension(IPhysicalDimension pdPhysicalDimension)
		{
			if (pdPhysicalDimension.ExponentOfUnit.Ampere != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Candela != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Kelvin != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Kilogram != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Metre != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Mole != 0)
				return false;

			if (pdPhysicalDimension.ExponentOfUnit.Second == 0)
				return false;

			guPhysicalDimensionId = pdPhysicalDimension.Id;
			return true;
		}

		private static ITimePeriod? Initialize(
			string sConcurrencyStamp,
			Guid guId,
			double[] dMagnitude,
			double dOffset,
			Guid guPhysicalDimensionId)
		{
			ITimePeriod pdTimePeriod = new TimePeriod(
				sConcurrencyStamp: sConcurrencyStamp,
				guId: guId,
				dMagnitude: dMagnitude,
				dOffset: dOffset,
				guPhysicalDimensionId: guPhysicalDimensionId);

			if (string.IsNullOrWhiteSpace(sConcurrencyStamp) == true)
				return null;

			if (guId == default)
				return null;

			if (guPhysicalDimensionId == default)
				return null;

			return pdTimePeriod;
		}

		public static ITimePeriod? Create(
			double[] dMagnitude,
			double dOffset,
			IPhysicalDimension pdPhysicalDimension)
		{
			ITimePeriod? pdTimePeriod = Initialize(
				sConcurrencyStamp: Guid.NewGuid().ToString(),
				guId: Guid.NewGuid(),
				dMagnitude: dMagnitude,
				dOffset: dOffset,
				guPhysicalDimensionId: pdPhysicalDimension.Id);

			if (pdTimePeriod is null)
				return null;

			if (pdTimePeriod.TryChangePhysicalDimension(pdPhysicalDimension) == false)
				return null;

			return pdTimePeriod;
		}

		public static ITimePeriod? Initialize(ITimePeriodTransferObject dtoTimePeriod)
		{
			return Initialize(
			sConcurrencyStamp: dtoTimePeriod.ConcurrencyStamp,
			guId: dtoTimePeriod.Id,
			dMagnitude: dtoTimePeriod.Magnitude,
			dOffset: dtoTimePeriod.Offset,
			guPhysicalDimensionId: dtoTimePeriod.PhysicalDimensionId);
		}

		/// <inheritdoc/>
		public ITimePeriodTransferObject WriteTo<T>() where T : ITimePeriodTransferObject, new()
		{
			return new T()
			{
				ConcurrencyStamp = sConcurrencyStamp,
				Id = guId,
				Magnitude = dMagnitude,
				Offset = dOffset,
				PhysicalDimensionId = guPhysicalDimensionId
			};
		}
	}
}
