using Application.Command.PhysicalData.PhysicalDimension.Update;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.UpdatePhysicalDimension
{
	public sealed class UpdatePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public UpdatePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			fxtPhysicalData = fxtPhysicalDimension;
			prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPhysicalDimensionIsUpdated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
			{
				ExponentOfAmpere = 0,
				ExponentOfCandela = 0,
				ExponentOfKelvin = 0,
				ExponentOfKilogram = 0,
				ExponentOfMetre = 1,
				ExponentOfMole = 0,
				ExponentOfSecond = 0,
				ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
				ConversionFactorToSI = 1,
				CultureName = "en-GB",
				Name = "Metre",
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				Symbol = "l",
				Unit = "m"
			};

			UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
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

			// Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
			{
				ExponentOfAmpere = 0,
				ExponentOfCandela = 0,
				ExponentOfKelvin = 0,
				ExponentOfKilogram = 0,
				ExponentOfMetre = 1,
				ExponentOfMole = 0,
				ExponentOfSecond = 0,
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				ConversionFactorToSI = 1,
				CultureName = "en-GB",
				Name = "Metre",
				PhysicalDimensionId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty,
				Symbol = "l",
				Unit = "m"
			};

			// Act
			UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});
		}

		[Fact]
		public async Task Update_ShouldReturnRepositoryError_WhenConcurrencyStampDoNotMatch()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			string sObsoleteConcurrencyStamp = Guid.NewGuid().ToString();

			UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
			{
				ExponentOfAmpere = 0,
				ExponentOfCandela = 0,
				ExponentOfKelvin = 0,
				ExponentOfKilogram = 0,
				ExponentOfMetre = 1,
				ExponentOfMole = 0,
				ExponentOfSecond = 0,
				ConcurrencyStamp = sObsoleteConcurrencyStamp,
				ConversionFactorToSI = 1,
				CultureName = "en-GB",
				Name = "Metre",
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				Symbol = "l",
				Unit = "m"
			};

			UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Should().Be(DefaultMessageError.ConcurrencyViolation);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});

			// Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

        [Fact]
		public async Task Update_ShouldReturnRepositoryError_WhenCultureNameIsNotValid()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			string sInvalidCultureName = "INVALID_CULTURE_NAME";

			UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
			{
				ExponentOfAmpere = 0,
				ExponentOfCandela = 0,
				ExponentOfKelvin = 0,
				ExponentOfKilogram = 0,
				ExponentOfMetre = 1,
				ExponentOfMole = 0,
				ExponentOfSecond = 0,
				ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
				ConversionFactorToSI = 1,
				CultureName = sInvalidCultureName,
				Name = "Metre",
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				Symbol = "l",
				Unit = "m"
			};

			// Act
			UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(DomainError.Code.Method);
					msgError.Description.Should().Be("Culture name is not valid.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});

			// Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}