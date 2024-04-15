using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportToken
{
    public class PassportTokenRepositorySpecification_VerifyCredentialAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportTokenRepositorySpecification_VerifyCredentialAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Verify_ShouldReturnAllowedAccessAttempt_WhenCredentialMatches()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<int> rsltVerify = await fxtAuthorizationData.PassportTokenRepository.VerifyCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltVerify.Match(
				msgError => false,
				iAllowedAccessAttempt =>
				{
					iAllowedAccessAttempt.Should().Be(fxtAuthorizationData.PassportSetting.MaximalAllowedAccessAttempt);
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Verify_ShouldReturnDecrementedAllowedAccessAttempt_WhenSignatureDoesNotMatch()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportCredential ppInvalidCredential = DataFaker.PassportCredential.Create(ppCredential.Credential, Guid.NewGuid().ToString());

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<int> rsltVerify = await fxtAuthorizationData.PassportTokenRepository.VerifyCredentialAsync(ppInvalidCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltVerify.Match(
				msgError => false,
				iAllowedAccessAttempt =>
				{
					iAllowedAccessAttempt.Should().Be(fxtAuthorizationData.PassportSetting.MaximalAllowedAccessAttempt + (-1));
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Verify_ShouldResetRefreshToken_WhenCredentialMatches()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			string sRefreshToken = string.Empty;

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassportToken> rsltTokenToVerify = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			rsltTokenToVerify.Match(
				msgError => false,
				ppTokenInRepository =>
				{
					sRefreshToken = ppTokenInRepository.RefreshToken;
					return true;
				});

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
					ppTokenInRepository.Should().NotBe(sRefreshToken);
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}