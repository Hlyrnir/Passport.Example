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
    public class PassportTokenRepositorySpecification_ResetCredentialAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportTokenRepositorySpecification_ResetCredentialAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Reset_ShouldReturnTrue_WhenCredentialIsReset()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.Create(ppCredential.Credential, Guid.NewGuid().ToString());

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltReset = await fxtAuthorizationData.PassportTokenRepository.ResetCredentialAsync(ppToken, ppCredentialToApply, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			await rsltReset.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async bResult =>
				{
					IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredentialToApply, prvTime.GetUtcNow(), CancellationToken.None);

					return rsltToken.Match(
						msgError =>
						{
							msgError.Should().BeNull();

							return false;
						},
						ppTokenInRepository =>
						{
							ppTokenInRepository.PassportId.Should().Be(ppPassport.Id);

							return true;
						});
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Reset_ShouldReturnRepositoryError_WhenCredentialDoesNotExist()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltReset = await fxtAuthorizationData.PassportTokenRepository.ResetCredentialAsync(ppToken, ppCredentialToApply, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltReset.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Credential has not been reset at provider {ppCredentialToApply.Provider}.");

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

		[Fact]
		public async Task Reset_ShouldReturnRepositoryError_WhenPassportIdDoesNotExist()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(Guid.NewGuid());
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.Create(ppCredential.Credential, ppCredential.Signature);

			Guid guPassportId = Guid.NewGuid();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltReset = await fxtAuthorizationData.PassportTokenRepository.ResetCredentialAsync(ppToken, ppCredentialToApply, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltReset.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Credential has not been reset at provider {ppCredentialToApply.Provider}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}
