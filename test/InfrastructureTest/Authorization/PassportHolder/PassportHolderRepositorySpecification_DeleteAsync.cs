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
    public class PassportHolderRepositorySpecification_DeleteAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportHolderRepositorySpecification_DeleteAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenHolderIsDeleted()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
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
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenConcurrencyStampIsDifferent()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			IPassportHolder ppHolderToDelete = DataFaker.PassportHolder.Clone(ppHolderToClone: ppHolder, ppSetting: fxtAuthorizationData.PassportSetting, bResetConcurrencyStamp: true);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolderToDelete, CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not delete holder {ppHolderToDelete.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}
	}
}