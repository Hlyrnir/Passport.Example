using Application.Common.Authorization;
using Application.Common.Error;
using Application.Filter;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.PhysicalData.PhysicalDimension.ByFilter;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;
using static ApplicationTest.Error.TestError;

namespace ApplicationTest.Query.PhysicalData.PhysicalDimension.ByFilter
{
    public sealed class PhysicalDimensionByFilterAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public PhysicalDimensionByFilterAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenPassportIdIsAuthorized()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Read);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

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

			IAuthorization<PhysicalDimensionByFilterQuery> hndlAuthorization = new PhysicalDimensionByFilterAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: qryByFilter,
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
		public async Task Read_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

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

			IAuthorization<PhysicalDimensionByFilterQuery> hndlAuthorization = new PhysicalDimensionByFilterAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: qryByFilter,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Code.Should().Be(Repository.PassportVisa.NotFound.Code);
					msgError.Description.Should().Contain(Repository.PassportVisa.NotFound.Description);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Theory]
		[InlineData(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Create)]
		[InlineData(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Delete)]
		[InlineData(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Read)]
		[InlineData(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Update)]
		public async Task Read_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
		{
			// Arrange
			IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.PhysicalDimension, AuthorizationDefault.Level.Read);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

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

			IAuthorization<PhysicalDimensionByFilterQuery> hndlAuthorization = new PhysicalDimensionByFilterAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: qryByFilter,
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
