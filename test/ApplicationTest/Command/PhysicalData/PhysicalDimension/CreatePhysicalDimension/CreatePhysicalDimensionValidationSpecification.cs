using Application.Command.PhysicalData.PhysicalDimension.Create;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.CreatePhysicalDimension
{
	public sealed class CreatePhysicalDimensionValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public CreatePhysicalDimensionValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			CreatePhysicalDimensionCommand cmdCreate = new CreatePhysicalDimensionCommand()
			{
				ExponentOfAmpere = 0,
				ExponentOfCandela = 0,
				ExponentOfKelvin = 0,
				ExponentOfKilogram = 0,
				ExponentOfMetre = 1,
				ExponentOfMole = 0,
				ExponentOfSecond = 0,
				ConversionFactorToSI = 1,
				CultureName = "en-GB",
				Name = "Metre",
				Symbol = "l",
				Unit = "m",
				RestrictedPassportId = Guid.Empty
			};

			IValidation<CreatePhysicalDimensionCommand> hndlValidation = new CreatePhysicalDimensionValidation();

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdCreate,
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

		//[Theory]
		//[InlineData(true, new double[] { double.MinValue, double.MinValue })]
		//[InlineData(true, new double[] { double.MaxValue, double.MaxValue })]
		//public async Task Create_ShouldReturnTrue_WhenMagnitudeIsValid(bool bExpectedResult, double[] dMagnitude)
		//{
		//	// Arrange
		//	CreatePhysicalDimensionCommand cmdCreate = new CreatePhysicalDimensionCommand()
		//	{
		//		ExponentOfAmpere = 0,
		//		ExponentOfCandela = 0,
		//		ExponentOfKelvin = 0,
		//		ExponentOfKilogram = 0,
		//		ExponentOfMetre = 1,
		//		ExponentOfMole = 0,
		//		ExponentOfSecond = 0,
		//		ConversionFactorToSI = 1,
		//		CultureName = "en-GB",
		//		Name = "Metre",
		//		Symbol = "l",
		//		Unit = "m",
		//		RestrictedPassportId = Guid.Empty
		//	};

		//	IValidation<CreatePhysicalDimensionCommand> hndlValidation = new CreatePhysicalDimensionValidation();

		//	// Act
		//	IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
		//		msgMessage: cmdCreate,
		//		tknCancellation: CancellationToken.None);

		//	// Assert
		//	rsltValidation.Match(
		//		msgError =>
		//		{
		//			msgError.Should().BeNull();

		//			return false;
		//		},
		//		bResult =>
		//		{
		//			bResult.Should().Be(bExpectedResult);

		//			return true;
		//		});
		//}

		//[Theory]
		//[InlineData(true, double.MinValue)]
		//[InlineData(true, double.MaxValue)]
		//public async Task Create_ShouldReturnTrue_WhenOffsetIsValid(bool bExpectedResult, double dOffset)
		//{
		//	// Arrange
		//	CreatePhysicalDimensionCommand cmdCreate = new CreatePhysicalDimensionCommand()
		//	{
		//		ExponentOfAmpere = 0,
		//		ExponentOfCandela = 0,
		//		ExponentOfKelvin = 0,
		//		ExponentOfKilogram = 0,
		//		ExponentOfMetre = 1,
		//		ExponentOfMole = 0,
		//		ExponentOfSecond = 0,
		//		ConversionFactorToSI = 1,
		//		CultureName = "en-GB",
		//		Name = "Metre",
		//		Symbol = "l",
		//		Unit = "m",
		//		RestrictedPassportId = Guid.Empty
		//	};

		//	IValidation<CreatePhysicalDimensionCommand> hndlValidation = new CreatePhysicalDimensionValidation();

		//	// Act
		//	IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
		//		msgMessage: cmdCreate,
		//		tknCancellation: CancellationToken.None);

		//	// Assert
		//	rsltValidation.Match(
		//		msgError =>
		//		{
		//			msgError.Should().BeNull();

		//			return false;
		//		},
		//		bResult =>
		//		{
		//			bResult.Should().Be(bExpectedResult);

		//			return true;
		//		});
		//}
	}
}
