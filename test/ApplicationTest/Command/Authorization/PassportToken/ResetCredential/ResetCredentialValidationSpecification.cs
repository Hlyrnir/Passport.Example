using Application.Command.Authorization.PassportToken.ResetCredential;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.ResetCredential
{
    public sealed class ResetCredentialValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public ResetCredentialValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenCredentialExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			string sInvalidSignature = $"Inval!d_{Guid.NewGuid()}";

			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.Create(ppCredentialToVerify.Credential, sInvalidSignature);

			ResetCredentialCommand cmdUpdate = new ResetCredentialCommand()
			{
				CredentialToApply = ppCredentialToApply,
				CredentialToVerify = ppCredentialToVerify,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<ResetCredentialCommand> hndlValidation = new ResetCredentialValidation(
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
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenCredentialDoesNotExist()
		{
			// Arrange
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();

			string sInvalidSignature = $"Inval!d_{Guid.NewGuid()}";

			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.Create(ppCredentialToVerify.Credential, sInvalidSignature);

			ResetCredentialCommand cmdUpdate = new ResetCredentialCommand()
			{
				CredentialToApply = ppCredentialToApply,
				CredentialToVerify = ppCredentialToVerify,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<ResetCredentialCommand> hndlValidation = new ResetCredentialValidation(
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
					msgError.Description.Should().Contain($"Credential {ppCredentialToVerify.Credential} does not exist.");

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