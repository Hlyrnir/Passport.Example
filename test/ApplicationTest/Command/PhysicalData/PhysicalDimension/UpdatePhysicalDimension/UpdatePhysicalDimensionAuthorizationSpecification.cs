using Application.Command.PhysicalData.PhysicalDimension.Update;
using Application.Common.Authorization;
using Application.Common.Error;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.UpdatePhysicalDimension
{
    public sealed class UpdatePhysicalDimensionAuthorizationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public UpdatePhysicalDimensionAuthorizationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportIdIsAuthorized()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Update);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

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

			IAuthorization<UpdatePhysicalDimensionCommand> hndlAuthorization = new UpdatePhysicalDimensionAuthorization(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdUpdate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
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

			//Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

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

			IAuthorization<UpdatePhysicalDimensionCommand> hndlAuthorization = new UpdatePhysicalDimensionAuthorization(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdUpdate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Code.Should().Be(TestError.Repository.PassportVisa.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportVisa.NotFound.Description);

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
		public async Task Update_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
		{
			// Arrange
			IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Update);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

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

			IAuthorization<UpdatePhysicalDimensionCommand> hndlAuthorization = new UpdatePhysicalDimensionAuthorization(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdUpdate,
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
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppUnauthorizedVisa, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppAuthorizedVisa, CancellationToken.None);
		}
	}
}