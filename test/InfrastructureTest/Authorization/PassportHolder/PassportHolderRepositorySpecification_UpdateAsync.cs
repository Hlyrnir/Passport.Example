using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportHolder
{
    public class PassportHolderRepositorySpecification_UpdateAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportHolderRepositorySpecification_UpdateAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportIsUpdated()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToUpdate = DataFaker.PassportHolder.Clone(ppHolderToClone: ppHolder, ppSetting: fxtAuthorizationData.PassportSetting);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppHolderToUpdate.TryChangeEmailAddress(sEmailAddress: "another@passport.org", ppSetting: fxtAuthorizationData.PassportSetting);

			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportHolderRepository.UpdateAsync(ppHolderToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
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
			IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolderToUpdate.Id, CancellationToken.None);

			await rsltHolder.MatchAsync(
				msgError => false,
				ppHolderToDelete => fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToDelete, CancellationToken.None));
		}

		[Fact]
		public async Task Update_ShouldChangeConcurrencyStamp_WhenPassportIsUpdated()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToUpdate = DataFaker.PassportHolder.Clone(ppHolderToClone: ppHolder, ppSetting: fxtAuthorizationData.PassportSetting);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			await fxtAuthorizationData.PassportHolderRepository.UpdateAsync(ppHolderToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassportHolder> rsltUpdate = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolderToUpdate.Id, CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.ConcurrencyStamp.Should().NotBe(ppHolderToUpdate.ConcurrencyStamp);
					ppPassportInRepository.CultureName.Should().Be(ppHolderToUpdate.CultureName);
					ppPassportInRepository.EmailAddress.Should().Be(ppHolderToUpdate.EmailAddress);
					ppPassportInRepository.EmailAddressIsConfirmed.Should().Be(ppHolderToUpdate.EmailAddressIsConfirmed);
					ppPassportInRepository.FirstName.Should().Be(ppHolderToUpdate.FirstName);
					ppPassportInRepository.Id.Should().Be(ppHolderToUpdate.Id);
					ppPassportInRepository.LastName.Should().Be(ppHolderToUpdate.LastName);
					ppPassportInRepository.PhoneNumber.Should().Be(ppHolderToUpdate.PhoneNumber);
					ppPassportInRepository.PhoneNumberIsConfirmed.Should().Be(ppHolderToUpdate.PhoneNumberIsConfirmed);
					ppPassportInRepository.SecurityStamp.Should().Be(ppHolderToUpdate.SecurityStamp);
					return true;
				});

			// Clean up
			IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolderToUpdate.Id, CancellationToken.None);

			await rsltHolder.MatchAsync(
				msgError => false,
				ppHolderToDelete => fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToDelete, CancellationToken.None));
		}

		[Fact]
		public async Task Update_ShouldReturnFalse_WhenConcurrencyStampIsDifferent()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToUpdate = DataFaker.PassportHolder.Clone(ppHolderToClone: ppHolder, ppSetting: fxtAuthorizationData.PassportSetting, bResetConcurrencyStamp: true);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportHolderRepository.UpdateAsync(ppHolderToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not update holder {ppHolderToUpdate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolderToUpdate.Id, CancellationToken.None);

			await rsltHolder.MatchAsync(
				msgError => false,
				ppHolderToDelete => fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToDelete, CancellationToken.None));
		}
	}
}