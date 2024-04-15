using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportVisa
{
    public class PassportVisaRepositorySpecification_FindByNameAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportVisaRepositorySpecification_FindByNameAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Find_ShouldFindVisa_WhenNameExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByNameAsync(ppVisa.Name, ppVisa.Level, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.Should().BeEquivalentTo(ppVisa);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Find_ShouldReturnRepositoryError_WhenVisaIdDoesNotExist()
		{
			// Arrange
			Guid guVisaId = Guid.NewGuid();

			// Act
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(guVisaId, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"No data for {guVisaId} has been found.");

					return false;
				},
				ppVisaInRepository =>
				{
					ppVisaInRepository.Should().BeNull();

					return true;
				});
		}

		[Fact]
		public async Task Find_ShouldReturnRepositoryError_WhenNameDoesNotExist()
		{
			// Arrange
			string sName = "DEFAULT_NAME";
			int iLevel = 0;

			// Act
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByNameAsync(sName, iLevel, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"No data for visa of name {sName} at level {iLevel} has been found.");

					return false;
				},
				ppVisaInRepository =>
				{
					ppVisaInRepository.Should().BeNull();

					return true;
				});
		}
	}
}
