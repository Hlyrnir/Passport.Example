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
    public class PassportHolderRepositorySpecification_InsertAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportHolderRepositorySpecification_InsertAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnTrue_WhenHolderIsCreated()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);

			// Act
			IRepositoryResult<bool> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltHolder.Match<bool>(
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
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnRepositoryError_WhenHolderIdExistsAndEmailAddressIsDifferent()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToCreate = DataFaker.PassportHolder.Clone(ppHolder, fxtAuthorizationData.PassportSetting, bResetConcurrencyStamp: true);
			ppHolderToCreate.TryChangeEmailAddress("another@passport.org", fxtAuthorizationData.PassportSetting);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolderToCreate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not create holder {ppHolderToCreate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToCreate, CancellationToken.None);
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnRepositoryError_WhenEmailAddressExistsAndHolderIdIsDifferent()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToCreate = DataFaker.PassportHolder.Clone(ppHolderToClone: ppHolder, ppSetting: fxtAuthorizationData.PassportSetting, bResetId: true);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolderToCreate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not create holder {ppHolderToCreate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToCreate, CancellationToken.None);
		}
	}
}