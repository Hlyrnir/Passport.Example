using Domain.Aggregate.PhysicalData.PhysicalDimension.ValueObject;
using Domain.Interface.Transfer;

namespace Domain.Interface.PhysicalData
{
    public interface IPhysicalDimension
    {
		/// <summary>
		/// Optimistic concurrency - No locks, but some risk of data loss
		/// </summary>
		string ConcurrencyStamp { get; }

		/// <summary>
		/// Unique identifier
		/// </summary>
        Guid Id { get; }

		/// <summary>
		/// Use the conversion factor to adjust for a deviation from the SI unit.
		/// </summary>
		double ConversionFactorToSI { get; set; }

		/// <summary>
		/// Specify the culture name of the language used for other properties.
		/// </summary>
		string CultureName { get; }

		/// <summary>
		/// Specify the exponent for each SI unit.
		/// 
		/// Example - Area:
		/// ExponentOfUnit.None = 0,
		/// ExponentOfUnit.Time = 0,
		/// ExponentOfUnit.Length = 2,
		/// ExponentOfUnit.Kilogram = 0,
		/// ExponentOfUnit.Ampere = 0,
		/// ExponentOfUnit.Kelvin = 0,
		/// ExponentOfUnit.Mole = 0,
		/// ExponentOfUnit.Candela = 0
		/// </summary>
		PhysicalUnitExponent ExponentOfUnit { get; }

		/// <summary>
		/// Example: Length
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Example: 'l'
		/// </summary>
		string Symbol { get; set; }

		/// <summary>
		/// Example: 'm'
		/// </summary>
		string Unit { get; set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="sCultureName"></param>
		/// <returns></returns>
        bool TryChangeCultureName(string sCultureName);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IPhysicalDimensionTransferObject WriteTo<T>() where T : IPhysicalDimensionTransferObject, new();
	}
}