using Application.Command.PhysicalData.PhysicalDimension.Create;
using Application.Common.Authorization;
using Application.Error;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;
using static ApplicationTest.Error.TestError;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.CreatePhysicalDimension
{
	public sealed class CreatePhysicalDimensionAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;

		private readonly ITimeProvider prvTime;

		public CreatePhysicalDimensionAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenPassportIdIsAuthorized()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Create);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

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

			IAuthorization<CreatePhysicalDimensionCommand> hndlAuthorization = new CreatePhysicalDimensionAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdCreate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return true;
				},
				bResult =>
				{
					bResult.Should().BeTrue();

					return true;
				});

			//Clean up
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Create_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

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

			IAuthorization<CreatePhysicalDimensionCommand> hndlAuthorization = new CreatePhysicalDimensionAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdCreate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Code.Should().Be(Repository.PassportVisa.NotFound.Code);
					msgError.Description.Should().Be(Repository.PassportVisa.NotFound.Description);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Theory]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Create)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Delete)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Read)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Update)]
		public async Task Create_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
		{
			// Arrange
			IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Create);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

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

			IAuthorization<CreatePhysicalDimensionCommand> hndlAuthorization = new CreatePhysicalDimensionAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdCreate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Should().Be(AuthorizationError.PassportVisa.VisaDoesNotExist);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppUnauthorizedVisa, CancellationToken.None);
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppAuthorizedVisa, CancellationToken.None);
		}
	}
}
