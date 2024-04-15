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
    public class PassportTokenRepositorySpecification_FindTokenByRefreshTokenAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportTokenRepositorySpecification_FindTokenByRefreshTokenAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Find_ShouldFindPassportToken_WhenRefreshTokenExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			await rsltToken.MatchAsync(
				msgError => false,
				async ppPassportTokenInRepository =>
				{
					// Act
					DateTimeOffset dtAttemptedAt = prvTime.GetUtcNow();
					IRepositoryResult<IPassportToken> rsltRefresh = await fxtAuthorizationData.PassportTokenRepository.FindTokenByRefreshTokenAsync(ppPassportTokenInRepository.PassportId, ppPassportTokenInRepository.Provider, ppPassportTokenInRepository.RefreshToken, dtAttemptedAt, CancellationToken.None);

					// Assert
					return rsltRefresh.Match(
						msgError =>
						{
							msgError.Should().BeNull();

							return false;
						},
						ppRefreshedPassportTokenInRepository =>
						{
							ppRefreshedPassportTokenInRepository.Should().NotBeNull();
							ppRefreshedPassportTokenInRepository.PassportId.Should().Be(ppPassport.Id);
							ppRefreshedPassportTokenInRepository.Provider.Should().Be(ppCredential.Provider);
							ppRefreshedPassportTokenInRepository.RefreshToken.Should().NotBe(ppPassportTokenInRepository.RefreshToken);
							ppRefreshedPassportTokenInRepository.TwoFactorIsEnabled.Should().Be(ppPassportTokenInRepository.TwoFactorIsEnabled);

							return true;
						});
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Find_ShouldReturnRepositoryError_WhenRefreshTokenIsInvalid()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			string sInvalidRefreshToken = Guid.NewGuid().ToString();
			// Act
			IRepositoryResult<IPassportToken> rsltRefresh = await fxtAuthorizationData.PassportTokenRepository.FindTokenByRefreshTokenAsync(ppPassport.Id, ppCredential.Provider, sInvalidRefreshToken, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltRefresh.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Refresh token does not match at provider {ppCredential.Provider}.");

					return false;
				},
				ppRefreshedPassportTokenInRepository =>
				{
					ppRefreshedPassportTokenInRepository.Should().BeNull();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}
