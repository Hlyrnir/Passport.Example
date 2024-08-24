using Application.Common.Error;
using Application.Filter;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.PhysicalData.PhysicalDimension.ByFilter;
using ApplicationTest.Common;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.PhysicalDimension.ByFilter
{
    public sealed class PhysicalDimensionByFilterValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public PhysicalDimensionByFilterValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenFilterIsValid()
		{
			// Arrange
			PhysicalDimensionByFilterQuery qryByFilter = new PhysicalDimensionByFilterQuery()
			{
				Filter = new PhysicalDimensionByFilterOption()
				{
					ConversionFactorToSI = null,
					CultureName = null,
					ExponentOfAmpere = null,
					ExponentOfCandela = null,
					ExponentOfKelvin = null,
					ExponentOfKilogram = null,
					ExponentOfMetre = null,
					ExponentOfMole = null,
					ExponentOfSecond = null,
					Name = null,
					Symbol = null,
					Unit = null,
					Page = 1,
					PageSize = 10
				},
				RestrictedPassportId = Guid.Empty
			};

			IValidation<PhysicalDimensionByFilterQuery> hndlValidation = new PhysicalDimensionByFilterValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryByFilter,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				bResult =>
				{
					bResult.Should().BeTrue();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenFilterContainsSqlStatement()
		{
			// Arrange
			PhysicalDimensionByFilterQuery qryByFilter = new PhysicalDimensionByFilterQuery()
			{
				Filter = new PhysicalDimensionByFilterOption()
				{
					ConversionFactorToSI = null,
					CultureName = null,
					ExponentOfAmpere = null,
					ExponentOfCandela = null,
					ExponentOfKelvin = null,
					ExponentOfKilogram = null,
					ExponentOfMetre = null,
					ExponentOfMole = null,
					ExponentOfSecond = null,
					Name = "SELECT",
					Symbol = null,
					Unit = null,
					Page = 1,
					PageSize = 10
				},
				RestrictedPassportId = Guid.Empty
			};

			IValidation<PhysicalDimensionByFilterQuery> hndlValidation = new PhysicalDimensionByFilterValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryByFilter,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Name contains forbidden statement.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}
	}
}
