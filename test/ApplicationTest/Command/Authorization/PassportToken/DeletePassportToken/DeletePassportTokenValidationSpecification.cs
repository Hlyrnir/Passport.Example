using Application.Command.Authorization.PassportToken.Delete;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.DeletePassportToken
{
    public sealed class DeletePassportTokenValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public DeletePassportTokenValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenTokenExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePassportTokenCommand cmdDelete = new DeletePassportTokenCommand()
			{
				CredentialToVerify = ppCredential,
				PassportTokenId = ppToken.Id,
				RestrictedPassportId = Guid.Empty
			};

			IValidation<DeletePassportTokenCommand> hndlValidation = new DeletePassportTokenValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
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
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnMessageError_WhenTokenDoesNotExist()
		{
			// Arrange
			DeletePassportTokenCommand cmdDelete = new DeletePassportTokenCommand()
			{
				CredentialToVerify = DataFaker.PassportCredential.CreateDefault(),
				PassportTokenId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IValidation<DeletePassportTokenCommand> hndlValidation = new DeletePassportTokenValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Passport token {cmdDelete.PassportTokenId} does not exist.");

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
