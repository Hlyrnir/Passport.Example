using Application.Command.Authorization.PassportToken.EnableTwoFactorAuthentication;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.EnableTwoFactorAuthentication
{
    public sealed class EnableTwoFactorAuthenticationValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public EnableTwoFactorAuthenticationValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create("default@ema.il", "$ignatUr3");
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			EnableTwoFactorAuthenticationCommand cmdUpdate = new EnableTwoFactorAuthenticationCommand()
			{
				CredentialToVerify = ppCredential,
				RestrictedPassportId = Guid.NewGuid(),
				TwoFactorIsEnabled = true
			};

			IValidation<EnableTwoFactorAuthenticationCommand> hndlValidation = new EnableTwoFactorAuthenticationValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
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
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenTokenDoesNotExist()
		{
			// Arrange
			EnableTwoFactorAuthenticationCommand cmdUpdate = new EnableTwoFactorAuthenticationCommand()
			{
				CredentialToVerify = DataFaker.PassportCredential.Create("default@ema.il", "$ignatUr3"),
				RestrictedPassportId = Guid.NewGuid(),
				TwoFactorIsEnabled = true
			};

			IValidation<EnableTwoFactorAuthenticationCommand> hndlValidation = new EnableTwoFactorAuthenticationValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Credential {cmdUpdate.CredentialToVerify.Credential} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}
	}
}