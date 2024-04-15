using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportToken
{
    public class PassportTokenRepositorySpecification_FindTokenByCredentialAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportTokenRepositorySpecification_FindTokenByCredentialAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindById_ShouldFindToken_WhenCredentialExistsAndIsEnabled()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltToken.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppTokenInRepository =>
				{
					ppTokenInRepository.Should().NotBeNull();
					ppTokenInRepository.PassportId.Should().Be(ppPassport.Id);
					ppTokenInRepository.Provider.Should().Be(ppCredential.Provider);
					ppTokenInRepository.RefreshToken.Should().NotBeNullOrWhiteSpace();
					ppTokenInRepository.TwoFactorIsEnabled.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task FindById_ShouldReturnRepositoryError_WhenCredentialDoesNotExist()
		{
			// Arrange
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			// Act
			IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltToken.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Credential and signature does not match at provider {ppCredential.Provider}.");

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().BeNull();

					return true;
				});
		}
	}
}