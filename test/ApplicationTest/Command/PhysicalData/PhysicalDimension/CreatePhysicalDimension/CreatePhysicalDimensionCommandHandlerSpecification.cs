using Application.Command.PhysicalData.PhysicalDimension.Create;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.CreatePhysicalDimension
{
	public sealed class CreatePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public CreatePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			fxtPhysicalData = fxtPhysicalDimension;
			prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionIsCreated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

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

			CreatePhysicalDimensionCommandHandler cmdHandler = new CreatePhysicalDimensionCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<Guid> rsltPhysicalDimensionId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			await rsltPhysicalDimensionId.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async guPhysicalDimensionId =>
				{
					IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByIdAsync(guPhysicalDimensionId, CancellationToken.None);

					rsltPhysicalDimension.Match(
						msgError =>
						{
							msgError.Should().BeNull();

							return false;
						},
						pdPhysicalDimension =>
						{
							pdPhysicalDimension.ExponentOfUnit.Ampere.Should().Be(cmdCreate.ExponentOfAmpere);
							pdPhysicalDimension.ExponentOfUnit.Candela.Should().Be(cmdCreate.ExponentOfCandela);
							pdPhysicalDimension.ExponentOfUnit.Kelvin.Should().Be(cmdCreate.ExponentOfKelvin);
							pdPhysicalDimension.ExponentOfUnit.Kilogram.Should().Be(cmdCreate.ExponentOfKilogram);
							pdPhysicalDimension.ExponentOfUnit.Metre.Should().Be(cmdCreate.ExponentOfMetre);
							pdPhysicalDimension.ExponentOfUnit.Mole.Should().Be(cmdCreate.ExponentOfMole);
							pdPhysicalDimension.ExponentOfUnit.Second.Should().Be(cmdCreate.ExponentOfSecond);
							pdPhysicalDimension.ConversionFactorToSI.Should().Be(cmdCreate.ConversionFactorToSI);
							pdPhysicalDimension.CultureName.Should().Be(cmdCreate.CultureName);
							pdPhysicalDimension.Name.Should().Be(cmdCreate.Name);
							pdPhysicalDimension.Symbol.Should().Be(cmdCreate.Symbol);
							pdPhysicalDimension.Unit.Should().Be(cmdCreate.Unit);

							return true;
						});

					//Clean up
					await rsltPhysicalDimension.MatchAsync(
						msgError => false,
						async pdPhysicalDimension => await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None));

					return true;
				});
		}

		//[Fact]
		//public async Task Create_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
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

		//	// Act
		//	CreatePhysicalDimensionCommandHandler cmdHandler = new CreatePhysicalDimensionCommandHandler(
		//		prvTime: prvTime,
		//		repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

		//	IMessageResult<Guid> rsltPhysicalDimensionId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

		//	// Assert
		//	rsltPhysicalDimensionId.Match(
		//		msgError =>
		//		{
		//			msgError.Should().NotBeNull();
		//			msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
		//			msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);
		//			return false;
		//		},
		//		guPhysicalDimensionId =>
		//		{
		//			guPhysicalDimensionId.Should().BeEmpty();

		//			return true;
		//		});
		//}
	}
}