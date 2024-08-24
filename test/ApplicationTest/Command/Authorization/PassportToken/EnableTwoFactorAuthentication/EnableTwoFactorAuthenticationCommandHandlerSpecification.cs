using Application.Command.Authorization.PassportToken.EnableTwoFactorAuthentication;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.EnableTwoFactorAuthentication
{
    public class EnableTwoFactorAuthenticationCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public EnableTwoFactorAuthenticationCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenTwoFactorIsEnabled()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			EnableTwoFactorAuthenticationCommand cmdUpdate = new EnableTwoFactorAuthenticationCommand()
			{
				CredentialToVerify = ppCredential,
				RestrictedPassportId = Guid.NewGuid(),
				TwoFactorIsEnabled = true
			};

			EnableTwoFactorAuthenticationCommandHandler hdlCommand = new EnableTwoFactorAuthenticationCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			// Act
			IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

			//Assert
			await rsltUpdate.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async bResult =>
				{
					bResult.Should().BeTrue();

					IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

					return rsltToken.Match(
						msgError => false,
						ppTokenInRepository =>
						{
							ppTokenInRepository.Id.Should().Be(ppToken.Id);
							ppTokenInRepository.PassportId.Should().Be(ppToken.PassportId);
							ppTokenInRepository.Provider.Should().Be(ppToken.Provider);
							ppTokenInRepository.RefreshToken.Should().NotBe(ppToken.RefreshToken);
							ppTokenInRepository.TwoFactorIsEnabled.Should().BeTrue();

							return true;
						});
				});

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenTwoFactorIsEnabled()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			ppToken.TryEnableTwoFactorAuthentication(true);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			EnableTwoFactorAuthenticationCommand cmdUpdate = new EnableTwoFactorAuthenticationCommand()
			{
				CredentialToVerify = ppCredential,
				RestrictedPassportId = Guid.NewGuid(),
				TwoFactorIsEnabled = true
			};

			EnableTwoFactorAuthenticationCommandHandler hdlCommand = new EnableTwoFactorAuthenticationCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			// Act
			IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

			//Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(DomainError.Code.Method);
					msgError.Description.Should().Be("Two factor authentication is already enabled.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}