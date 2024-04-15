using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.Passport
{
    public class PassportRepositorySpecification_DeleteAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportRepositorySpecification_DeleteAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnTrue_WhenPassportIsDeleted()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);

			// Assert
			rsltPassport.Match(
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
		public async Task DeleteAsync_ShouldReturnRepositoryError_WhenConcurrencyStampIsDifferent()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassport ppPassportToDelete = DataFaker.Passport.Clone(ppPassportToClone: ppPassport, bResetConcurrencyStamp: true);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassportToDelete, CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not delete passport {ppPassportToDelete.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}
