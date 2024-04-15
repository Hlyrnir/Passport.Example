using Domain.Aggregate.PhysicalData.PhysicalDimension.ValueObject;
using Domain.Interface.PhysicalData;
using Domain.Interface.Transfer;

namespace Domain.Aggregate.PhysicalData.PhysicalDimension
{
    public sealed class PhysicalDimension : IPhysicalDimension
    {
        private string sConcurrencyStamp;
        private Guid guId;

        private PhysicalUnitExponent pdExponentOfUnit;
        private double dConversionFactorToSI;
        private string sCultureName;
        private string sName;
        private string sSymbol;
        private string sUnit;

        private PhysicalDimension(
            float fExponentOfAmpere,
            float fExponentOfCandela,
            float fExponentOfKelvin,
            float fExponentOfKilogram,
            float fExponentOfMetre,
            float fExponentOfMole,
            float fExponentOfSecond,
            string sConcurrencyStamp,
            double dConversionFactorToSI,
            string sCultureName,
            Guid guId,
            string sName,
            string sSymbol,
            string sUnit)
        {
            this.pdExponentOfUnit = new PhysicalUnitExponent(
                fAmpere: fExponentOfAmpere,
                fCandela: fExponentOfCandela,
                fKelvin: fExponentOfKelvin,
                fKilogram: fExponentOfKilogram,
                fMetre: fExponentOfMetre,
                fMole: fExponentOfMole,
                fSecond: fExponentOfSecond);

            this.sConcurrencyStamp = sConcurrencyStamp;
            this.dConversionFactorToSI = dConversionFactorToSI;
            this.sCultureName = sCultureName;
            this.guId = guId;
            this.sName = sName;
            this.sSymbol = sSymbol;
            this.sUnit = sUnit;
        }

		/// <inheritdoc/>
		public string ConcurrencyStamp { get => sConcurrencyStamp; }

		/// <inheritdoc/>
		public Guid Id { get => guId; }

		/// <inheritdoc/>
		public PhysicalUnitExponent ExponentOfUnit { get => pdExponentOfUnit; private set => pdExponentOfUnit = value; }

		/// <inheritdoc/>
		public double ConversionFactorToSI { get => dConversionFactorToSI; set => dConversionFactorToSI = value; }

		/// <inheritdoc/>
		public string CultureName { get => sCultureName; private set => sCultureName = value; }

		/// <inheritdoc/>
		public string Name { get => sName; set => sName = value; }

		/// <inheritdoc/>
		public string Symbol { get => sSymbol; set => sSymbol = value; }

		/// <inheritdoc/>
		public string Unit { get => sUnit; set => sUnit = value; }

		/// <inheritdoc/>
		private const string sNumbers = "0123456789";
		public bool TryChangeCultureName(string sCultureName)
        {
			if (sCultureName.Length != 5)
				return false;

            Span<char> cNormalizedCultureName = sCultureName.ToCharArray();

			if (cNormalizedCultureName.IndexOfAny(sNumbers.AsSpan()) != (-1))
                return false;

            cNormalizedCultureName[0] = char.ToLowerInvariant(cNormalizedCultureName[0]);
            cNormalizedCultureName[1] = char.ToLowerInvariant(cNormalizedCultureName[1]);
            cNormalizedCultureName[2] = '-';
            cNormalizedCultureName[3] = char.ToUpperInvariant(cNormalizedCultureName[3]);
            cNormalizedCultureName[4] = char.ToUpperInvariant(cNormalizedCultureName[4]);

            this.sCultureName = cNormalizedCultureName.ToString();

            return true;
        }

		/// <inheritdoc/>
		private static IPhysicalDimension? Initialize(
			float fExponentOfAmpere,
			float fExponentOfCandela,
			float fExponentOfKelvin,
			float fExponentOfKilogram,
			float fExponentOfMetre,
			float fExponentOfMole,
			float fExponentOfSecond,
			string sConcurrencyStamp,
			double dConversionFactorToSI,
			string sCultureName,
			Guid guId,
			string sName,
			string sSymbol,
			string sUnit)
		{
			IPhysicalDimension pdPhysicalDimension = new PhysicalDimension(
				fExponentOfAmpere: fExponentOfAmpere,
				fExponentOfCandela: fExponentOfCandela,
				fExponentOfKelvin: fExponentOfKelvin,
				fExponentOfKilogram: fExponentOfKilogram,
				fExponentOfMetre: fExponentOfMetre,
				fExponentOfMole: fExponentOfMole,
				fExponentOfSecond: fExponentOfSecond,
				sConcurrencyStamp: sConcurrencyStamp,
				dConversionFactorToSI: dConversionFactorToSI,
				sCultureName: sCultureName,
				guId: guId,
				sName: sName,
				sSymbol: sSymbol,
				sUnit: sUnit);

			if (string.IsNullOrWhiteSpace(sConcurrencyStamp) == true)
				return null;

			if (dConversionFactorToSI < 0)
				return null;

			if (guId == default)
				return null;

			if (string.IsNullOrWhiteSpace(sName) == true)
				return null;

			if (string.IsNullOrWhiteSpace(sSymbol) == true)
				return null;

			if (string.IsNullOrWhiteSpace(sUnit) == true)
				return null;

			if (pdPhysicalDimension.TryChangeCultureName(sCultureName) == false)
				return null;

			return pdPhysicalDimension;
		}

		public static IPhysicalDimension? Create(
			float fExponentOfAmpere,
			float fExponentOfCandela,
			float fExponentOfKelvin,
			float fExponentOfKilogram,
			float fExponentOfMetre,
			float fExponentOfMole,
			float fExponentOfSecond,
            double dConversionFactorToSI,
            string sCultureName,
            string sName,
            string sSymbol,
            string sUnit)
        {
            return Initialize(
                fExponentOfAmpere: fExponentOfAmpere,
                fExponentOfCandela: fExponentOfCandela,
                fExponentOfKelvin: fExponentOfKelvin,
                fExponentOfKilogram: fExponentOfKilogram,
                fExponentOfMetre: fExponentOfMetre,
                fExponentOfMole: fExponentOfMole,
                fExponentOfSecond: fExponentOfSecond,
                sConcurrencyStamp: Guid.NewGuid().ToString(),
                dConversionFactorToSI: dConversionFactorToSI,
                sCultureName: sCultureName,
                guId: Guid.NewGuid(),
                sName: sName,
                sSymbol: sSymbol,
                sUnit: sUnit);
        }

		public static IPhysicalDimension? Initialize(IPhysicalDimensionTransferObject dtoPhysicalDimension)
		{
			return Initialize(
			fExponentOfAmpere: dtoPhysicalDimension.ExponentOfAmpere,
			fExponentOfCandela: dtoPhysicalDimension.ExponentOfCandela,
			fExponentOfKelvin: dtoPhysicalDimension.ExponentOfKelvin,
			fExponentOfKilogram: dtoPhysicalDimension.ExponentOfKilogram,
			fExponentOfMetre: dtoPhysicalDimension.ExponentOfMetre,
			fExponentOfMole: dtoPhysicalDimension.ExponentOfMole,
			fExponentOfSecond: dtoPhysicalDimension.ExponentOfSecond,
			sConcurrencyStamp: dtoPhysicalDimension.ConcurrencyStamp,
			dConversionFactorToSI: dtoPhysicalDimension.ConversionFactorToSI,
			sCultureName: dtoPhysicalDimension.CultureName,
			guId: dtoPhysicalDimension.Id,
			sName: dtoPhysicalDimension.Name,
			sSymbol: dtoPhysicalDimension.Symbol,
			sUnit: dtoPhysicalDimension.Unit);
		}

		/// <inheritdoc/>
		public IPhysicalDimensionTransferObject WriteTo<T>() where T : IPhysicalDimensionTransferObject, new()
		{
			return new T()
			{
				ExponentOfAmpere = pdExponentOfUnit.Ampere,
				ExponentOfCandela = pdExponentOfUnit.Candela,
				ExponentOfKelvin = pdExponentOfUnit.Kelvin,
				ExponentOfKilogram = pdExponentOfUnit.Kilogram,
				ExponentOfMetre = pdExponentOfUnit.Metre,
				ExponentOfMole = pdExponentOfUnit.Mole,
				ExponentOfSecond = pdExponentOfUnit.Second,
				ConcurrencyStamp = sConcurrencyStamp,
				ConversionFactorToSI = dConversionFactorToSI,
				CultureName = sCultureName,
				Id = guId,
				Name = sName,
				Symbol = sSymbol,
				Unit = sUnit
			};
		}
	}
}