using Application.Command.Authorization.PassportToken.ResetCredential;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.ResetCredential
{
    public class ResetCredentialCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public ResetCredentialCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportCredentialIsReset()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.CreateDefault();

			ResetCredentialCommand cmdUpdate = new ResetCredentialCommand()
			{
				CredentialToApply = ppCredentialToApply,
				CredentialToVerify = ppCredentialToVerify,
				RestrictedPassportId = Guid.NewGuid()
			};

			ResetCredentialCommandHandler hdlCommand = new ResetCredentialCommandHandler(
				prvTime: prvTime,
				uowUnitOfWork: fxtAuthorizationData.UnitOfWork,
				ppSetting: fxtAuthorizationData.PassportSetting,
				repoPassport: fxtAuthorizationData.PassportRepository,
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

					IRepositoryResult<int> rsltToken = await fxtAuthorizationData.PassportTokenRepository.VerifyCredentialAsync(ppCredentialToApply, prvTime.GetUtcNow(), CancellationToken.None);

					return rsltToken.Match(
						msgError => false,
						iNumberOfRemainingAttempts =>
						{
							iNumberOfRemainingAttempts.Should().Be(fxtAuthorizationData.PassportSetting.MaximalAllowedAccessAttempt);

							return true;
						});
				});

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}