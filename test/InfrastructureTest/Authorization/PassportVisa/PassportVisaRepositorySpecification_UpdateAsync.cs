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
    public class PassportVisaRepositorySpecification_UpdateAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportVisaRepositorySpecification_UpdateAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenVisaIsUpdated()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisaToUpdate = DataFaker.PassportVisa.Clone(ppVisaToClone: ppVisa);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppVisaToUpdate.TryChangeLevel(1);

			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportVisaRepository.UpdateAsync(ppVisaToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

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
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(ppVisaToUpdate.Id, CancellationToken.None);

			await rsltVisa.MatchAsync(
				msgError => false,
				ppVisa => fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None));
		}

		[Fact]
		public async Task Update_ShouldChangeConcurrencyStamp_WhenPassportIsUpdated()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisaToUpdate = DataFaker.PassportVisa.Clone(ppVisaToClone: ppVisa);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppVisaToUpdate.TryChangeLevel(1);

			await fxtAuthorizationData.PassportVisaRepository.UpdateAsync(ppVisaToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassportVisa> rsltUpdate = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(ppVisaToUpdate.Id, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppVisaInRepository =>
				{
					ppVisaInRepository.Should().NotBeNull();
					ppVisaInRepository.ConcurrencyStamp.Should().NotBe(ppVisaToUpdate.ConcurrencyStamp);
					ppVisaInRepository.Id.Should().Be(ppVisaToUpdate.Id);
					ppVisaInRepository.Level.Should().Be(ppVisaToUpdate.Level);
					ppVisaInRepository.Name.Should().Be(ppVisaToUpdate.Name);

					return true;
				});

			// Clean up
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(ppVisaToUpdate.Id, CancellationToken.None);

			await rsltVisa.MatchAsync(
				msgError => false,
				ppVisa => fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None));
		}

		[Fact]
		public async Task Update_ShouldReturnFalse_WhenConcurrencyStampIsDifferent()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisaToUpdate = DataFaker.PassportVisa.Clone(ppVisaToClone: ppVisa, bResetConcurrencyStamp: true);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportVisaRepository.UpdateAsync(ppVisaToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not update visa {ppVisaToUpdate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			IRepositoryResult<IPassportVisa> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(ppVisaToUpdate.Id, CancellationToken.None);

			await rsltVisa.MatchAsync(
				msgError => false,
				ppVisa => fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None));
		}
	}
}