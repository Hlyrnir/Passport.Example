using Application.Command.Authorization.PassportToken.Create;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.CreatePassportToken
{
    public sealed class CreatePassportTokenCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public CreatePassportTokenCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenTokenIsCreated()
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

			CreatePassportTokenCommandHandler cmdHandler = new CreatePassportTokenCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			// Act
			IMessageResult<Guid> rsltTokenId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			await rsltTokenId.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async guPassportTokenId =>
				{
					IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredentialToAdd, prvTime.GetUtcNow(), CancellationToken.None);

					return rsltToken.Match(
						msgError =>
						{
							msgError.Should().BeNull();

							return false;
						},
						ppTokenInRepository =>
						{
							ppTokenInRepository.Id.Should().Be(guPassportTokenId);

							return true;
						});
				});

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Create_ShouldReturnRepositoryError_WhenTokenIsNotCreated()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToAdd = DataFaker.PassportCredential.CreateDefault();

			CreatePassportTokenCommand cmdCreate = new CreatePassportTokenCommand()
			{
				CredentialToVerify = ppCredentialToVerify,
				CredentialToAdd = ppCredentialToAdd,
				RestrictedPassportId = Guid.NewGuid()
			};

			// Act
			CreatePassportTokenCommandHandler cmdHandler = new CreatePassportTokenCommandHandler(
				prvTime: prvTime,
				repoToken: fxtAuthorizationData.PassportTokenRepository);

			IMessageResult<Guid> rsltTokenId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			rsltTokenId.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(DomainError.Code.Method);
					msgError.Description.Should().Be($"Token at provider {cmdCreate.CredentialToAdd.Provider} does already exist.");
					return false;
				},
				guPassportVisaId =>
				{
					guPassportVisaId.Should().BeEmpty();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}