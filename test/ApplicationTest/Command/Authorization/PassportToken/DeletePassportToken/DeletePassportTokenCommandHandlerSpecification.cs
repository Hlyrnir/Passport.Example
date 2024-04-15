using Application.Command.Authorization.PassportToken.DeletePassportToken;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.DeletePassportToken
{
    public sealed class DeletePassportTokenCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public DeletePassportTokenCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenTokenIsDeleted()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppInitialCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportToken ppInitialToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppInitialToken, ppInitialCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppAdditionalCredential = DataFaker.PassportCredential.Create(ppInitialCredential.Credential, "another_$ignatUr3");
			IPassportToken ppAdditionalToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppAdditionalToken, ppAdditionalCredential, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePassportTokenCommand cmdDelete = new DeletePassportTokenCommand()
			{
				CredentialToVerify = ppAdditionalCredential,
				PassportTokenId = ppAdditionalToken.Id,
				RestrictedPassportId = Guid.Empty
			};

			DeletePassportTokenCommandHandler cmdHandler = new DeletePassportTokenCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			// Act
			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

			// Assert
			rsltDelete.Match(
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
		public async Task Delete_ShouldReturnRepositoryError_WhenTokenIdDoesNotExist()
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
				PassportTokenId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			// Act
			DeletePassportTokenCommandHandler cmdHandler = new DeletePassportTokenCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

			// Assert
			rsltDelete.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(DomainError.Code.Method);
					msgError.Description.Should().Be($"Token identifier {cmdDelete.PassportTokenId} does not match with credential.");
					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenCredentialDoesNotMatch()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			IPassportCredential ppWrongCredential = DataFaker.PassportCredential.Create(ppCredential.Credential, Guid.NewGuid().ToString());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePassportTokenCommand cmdDelete = new DeletePassportTokenCommand()
			{
				CredentialToVerify = ppWrongCredential,
				PassportTokenId = ppToken.Id,
				RestrictedPassportId = Guid.Empty
			};

			// Act
			DeletePassportTokenCommandHandler cmdHandler = new DeletePassportTokenCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

			// Assert
			rsltDelete.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Description);
					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}