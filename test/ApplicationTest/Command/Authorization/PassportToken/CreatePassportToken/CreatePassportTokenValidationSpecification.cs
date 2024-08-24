using Application.Command.Authorization.PassportToken.Create;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.CreatePassportToken
{
    public sealed class CreatePassportTokenValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public CreatePassportTokenValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenTokenDoesNotExist()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToAdd = DataFaker.PassportCredential.CreateAtProvider(
				sCredential: ppCredentialToVerify.Credential,
				sProvider: "DEFAULT_UNDEFINED",
				sSignature: ppCredentialToVerify.Signature);

			CreatePassportTokenCommand cmdCreate = new CreatePassportTokenCommand()
			{
				CredentialToVerify = ppCredentialToVerify,
				CredentialToAdd = ppCredentialToAdd,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<CreatePassportTokenCommand> hndlValidation = new CreatePassportTokenValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdCreate,
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
		public async Task Create_ShouldReturnMessageError_WhenTokenExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToAdd = ppCredentialToVerify;

			CreatePassportTokenCommand cmdCreate = new CreatePassportTokenCommand()
			{
				CredentialToVerify = ppCredentialToVerify,
				CredentialToAdd = ppCredentialToAdd,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<CreatePassportTokenCommand> hndlValidation = new CreatePassportTokenValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdCreate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Credential {cmdCreate.CredentialToAdd.Credential} at {cmdCreate.CredentialToAdd.Provider} does already exist.");

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