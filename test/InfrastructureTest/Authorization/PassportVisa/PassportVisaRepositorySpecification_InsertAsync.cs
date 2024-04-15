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
    public class PassportVisaRepositorySpecification_InsertAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportVisaRepositorySpecification_InsertAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnTrue_WhenVisaIsCreated()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			// Act
			IRepositoryResult<bool> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltVisa.Match(
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
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnRepositoryError_WhenPassportVisaIdExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisaToCreate = DataFaker.PassportVisa.Clone(ppVisa);
			ppVisaToCreate.TryChangeName("ANOTHER_DEFAULT");
			ppVisaToCreate.TryChangeLevel(1);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisaToCreate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPassport.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not create visa {ppVisaToCreate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnRepositoryError_WhenNameAndLevelExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisaToCreate = DataFaker.PassportVisa.CreateDefault();
			bool bNameIsChanged = ppVisaToCreate.TryChangeName(ppVisa.Name);
			bool bLevelIsChanged = ppVisaToCreate.TryChangeLevel(ppVisa.Level);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisaToCreate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			bNameIsChanged.Should().BeTrue();
			bLevelIsChanged.Should().BeTrue();

			rsltPassport.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not create visa {ppVisaToCreate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}
	}
}