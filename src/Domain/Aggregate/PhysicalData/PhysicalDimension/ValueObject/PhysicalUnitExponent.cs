namespace Domain.Aggregate.PhysicalData.PhysicalDimension.ValueObject
{
    public enum StandardUnit : byte
    {
        None,
        Time,
        Length,
        Mass,
        ElectricCurrent,
        ThermodynamicTemperature,
        AmountOfSubstance,
        LuminousIntensity
    }

    public struct PhysicalUnitExponent
    {
		private float fAmpere;
		private float fCandela;
		private float fKelvin;
		private float fKilogram;
		private float fMetre;
		private float fMole;
		private float fSecond;

        public PhysicalUnitExponent(
			float fAmpere =0,
			float fCandela =0,
			float fKelvin =0,
			float fKilogram =0,
			float fMetre =0,
			float fMole =0,
			float fSecond =0)
        {
            this.fAmpere = fAmpere;
            this.fCandela = fCandela;
            this.fKelvin = fKelvin;
            this.fKilogram = fKilogram;
            this.fMetre = fMetre;
            this.fMole = fMole;
            this.fSecond = fSecond;
        }

        public PhysicalUnitExponent(StandardUnit enumStandardUnit)
        {
            switch (enumStandardUnit)
            {
                case StandardUnit.None:
                    goto default;
                case StandardUnit.Time:
                    fAmpere = 0; fCandela = 0; fKelvin = 0; fKilogram = 0; fMetre = 0; fMole = 0; fSecond = 1;
                    break;
                case StandardUnit.Length:
					fAmpere = 0; fCandela = 0; fKelvin = 0; fKilogram = 0; fMetre = 1; fMole = 0; fSecond = 0;
					break;
                case StandardUnit.Mass:
					fAmpere = 0; fCandela = 0; fKelvin = 0; fKilogram = 1; fMetre = 0; fMole = 0; fSecond = 0;
					break;
                case StandardUnit.ElectricCurrent:
					fAmpere = 1; fCandela = 0; fKelvin = 0; fKilogram = 0; fMetre = 0; fMole = 0; fSecond = 0;
					break;
                case StandardUnit.ThermodynamicTemperature:
					fAmpere = 0; fCandela = 0; fKelvin = 1; fKilogram = 0; fMetre = 0; fMole = 0; fSecond = 0;
					break;
                case StandardUnit.AmountOfSubstance:
					fAmpere = 0; fCandela = 0; fKelvin = 0; fKilogram = 0; fMetre = 0; fMole = 1; fSecond = 0;
					break;
                case StandardUnit.LuminousIntensity:
					fAmpere = 0; fCandela = 1; fKelvin = 0; fKilogram = 0; fMetre = 0; fMole = 0; fSecond = 0;
					break;
                default:
					fAmpere = 0; fCandela = 0; fKelvin = 0; fKilogram = 0; fMetre = 0; fMole = 0; fSecond = 0;
					break;
            }
        }

        public float Second { get => fSecond; }

        public float Metre { get => fMetre; }

        public float Kilogram { get => fKilogram; }

        public float Ampere { get => fAmpere; }

        public float Kelvin { get => fKelvin; }

        public float Mole { get => fMole; }

        public float Candela { get => fCandela; }
    }
}
